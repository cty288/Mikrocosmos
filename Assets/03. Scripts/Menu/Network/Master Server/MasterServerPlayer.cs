using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MikrocosmosDatabase;
using Mirror;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using UnityEngine;
using UnityEngine.Events;
using EntityKey = PlayFab.MultiplayerModels.EntityKey;

public class MasterServerPlayer : NetworkBehaviour {
    [SyncVar] 
    private long timeIntervalPerCommand = 300;

    [SyncVar] private long lastCommandTick = 0;


    [SyncVar]
    [SerializeField]
    private string entityId;
    

    [SyncVar] 
    private Mode requestingMode;

    [SyncVar] 
    private PlayerTeamInfo teamInfo;
    public PlayerTeamInfo TeamInfo => teamInfo;


    #region Server


    //only server player saves this
    private GameMatch match;



    public Action<MasterServerPlayer> onPlayerDisconnect;
    public override void OnStopServer() {
        onPlayerDisconnect?.Invoke(this);
        RpcOnServerPlayerStop(teamInfo);
        ServerExitLobby();
        EventCenter.Broadcast(EventType.MENU_OnServerPlayerDisconnected,this);
    }

    [Command]
    private void CmdUpdateLastCommandTick() {
        lastCommandTick = DateTime.Now.Ticks;
    }

    [Command]
    private void CmdServerRequestMatch(Mode mode) {
        match = ((MasterServerNetworkManager) NetworkManager.singleton).ServerRequestFindAvailableMatch(mode);
        requestingMode = mode;
        if (match != null) {
            teamInfo.matchId = match.MatchId;

            if (match.JoinPlayer(this,teamInfo)) {
                match.teamInfoUpdate += ServerUpdateTeamInfo;
                match.onMatchStateChange += ServerUpdateMatchState;
                match.countdownUpdate += ServerUpdateMatchCountdown;
                TargetOnMatchReadyToGet();
                StartCoroutine(WaitWhileGetMatch());
            }
            else {
                ClientOnMatchmakingError(new PlayFabError{Error = PlayFabErrorCode.MatchmakingTicketMembershipLimitExceeded});
            }
            
        }
        else {
            //Playfab start matchmaking. This need to be called on the Client due to PlayFab Rate Limit
            TargetStartPlayFabMatchmaking(connectionToClient);
        }
    }

    [Server]
    private IEnumerator WaitWhileGetMatch() {
        yield return new WaitForSeconds(4f);
        TargetOnServerGetMatch(teamInfo);
    }


    [Command]
    private void CmdOnGetMatch(GetMatchResult result)
    {
        match =
            ((MasterServerNetworkManager) NetworkManager.singleton).ServerRequestNewPlayfabMatchmakingRoom(
                GameMode.GetGameModeObj(requestingMode), result.MatchId);
        
        if (match != null) {
            teamInfo.matchId = match.MatchId;
            if (match.JoinPlayer(this,teamInfo)) {
                match.teamInfoUpdate += ServerUpdateTeamInfo;
                match.onMatchStateChange += ServerUpdateMatchState;
                match.countdownUpdate += ServerUpdateMatchCountdown;
                TargetOnServerGetMatch(teamInfo);
            }
            else {
                TargetOnServerFailedToGetMatch();
            }
            
        }
        else {
            TargetOnServerFailedToGetMatch();
        }
    }


    /*[ServerCallback]
    private void ServerOnNewPlayerJoinLobby(MasterServerPlayer player,PlayerTeamInfo teamInfo) {
        TargetOnNewPlayerJoinsLobby(teamInfo);   
    }*/

    private void ServerUpdateTeamInfo(PlayerTeamInfo[] teamInfo) {
        //print("Server updating team info "+teamInfo.Length);
        TargetOnLobbyInfoUpdated(teamInfo,this.teamInfo);
    }

    [Server]
    private void ServerUpdateMatchState(MatchState matchState,GameMatch match) {
        TargetOnLobbyStateUpdated(matchState,match.Ip,match.Port);

        if (matchState == MatchState.StartingGameProcess || matchState == MatchState.GameAlreadyStart) {
            ServerDatabaseManager.Singleton.AddMatchIdToDatabase(teamInfo,match.MatchId);
        }
    }

    private void ServerUpdateMatchCountdown(float countDown) {
        TargetUpdateCountDown(countDown);
    }


    [Server]
    private void ServerExitLobby() {
        if (match) {
            match.teamInfoUpdate -= ServerUpdateTeamInfo;
            match.onMatchStateChange -= ServerUpdateMatchState;
            match.countdownUpdate -= ServerUpdateMatchCountdown;
            match = null;
        }
    }
    [Command]
    private void CmdAuthenticatePlayfabToken(PlayfabToken token) {
        ServerDatabaseManager.Singleton.AuthenticatePlayfabToken(token, () => {
            Debug.Log($"[MasterServerPlayer - {token.PlayerName}]: {token.Username} authenticate success on the database!");
            this.entityId = token.EntityId;
            this.teamInfo = new PlayerTeamInfo(token.PlayerName, -1, "", token.Username);
            EventCenter.Broadcast(EventType.MENU_OnServerPlayerAdded, this);
            TargetOnAuthenticateSuccess();
        }, () => {
            Debug.Log($"[MasterServerPlayer - {token.PlayerName}]:{token.Username} authenticate failed on the database!");
            OnStopServer();
            TargetKicked();
        });

    }


    [Command]
    private void CmdRequestLeaveLobby() {
        if (match == null) {
            TargetOnLeaveLobbyFailed(MatchError.UnableToFindMatch);
            return;
        }

        MatchError result = match.LeaveMatch(this);
        
        if (result == MatchError.NoError) {
            ServerExitLobby();
            TargetOnLeaveLobbySuccess();
        }
        else {
            TargetOnLeaveLobbyFailed(result);
        }

    }

    #endregion

    #region Client

    /// <summary>
    /// Call this to run Cmd functions. This function ensures the player is not requesting the server too frequently
    /// </summary>
    /// <param name="serverCommand"></param>
    [Client]
    private void RunServerCommand(Action serverCommand) {
        if (hasAuthority) {
            TimeSpan startSpan = new TimeSpan(lastCommandTick);
            TimeSpan nowSpan = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();

            if (subTimer.TotalMilliseconds > timeIntervalPerCommand)
            {
                serverCommand.Invoke();
                CmdUpdateLastCommandTick();
            }
            else
            {
                EventCenter.Broadcast<string, UnityAction, string, object[]>(EventType.MENU_Error,
                    "MENU_REQUEST_EXCEED", () => { }, "GAME_ACTION_CLOSE", null);
            }
        }
    }
    /// <summary>
    /// Call this to run Cmd functions. This function ensures the player is not requesting the server too frequently
    /// </summary>
    /// <typeparam name="T1">Argument of the function</typeparam>
    /// <typeparam name="T2">Return type of this function</typeparam>
    /// <param name="serverCommand">Server Cmd Function</param>
    /// <param name="arg">Cmd function argument</param>
    /// <returns></returns>
    [Client]
    private void RunServerCommand<T1>(Action<T1> serverCommand,T1 arg)
    {
        if (hasAuthority)
        {
            TimeSpan startSpan = new TimeSpan(lastCommandTick);
            TimeSpan nowSpan = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();

            if (subTimer.TotalMilliseconds > timeIntervalPerCommand)
            {
                CmdUpdateLastCommandTick();
                serverCommand.Invoke(arg);
            }
            else
            {
                EventCenter.Broadcast<string, UnityAction, string, object[]>(EventType.MENU_Error,
                    "MENU_REQUEST_EXCEED", () => { }, "GAME_ACTION_CLOSE", null);
            }
        }
    }

    public override void OnStartAuthority() {
        //改一下，等到身份验证成功才显示主菜单
        base.OnStartAuthority();
        EventCenter.AddListener(EventType.MENU_MATCHMAKING_ClientMatchmakingCancelled,CancelMatchmaking);
        if (PlayfabTokenPasser._instance) {
            CmdAuthenticatePlayfabToken(PlayfabTokenPasser._instance.Token);
        }
       
    }

    [TargetRpc]
    private void TargetOnAuthenticateSuccess() {
        EventCenter.Broadcast(EventType.MENU_AuthorityOnConnected);
    }

    public override void OnStopAuthority() {
        base.OnStopAuthority();
        EventCenter.RemoveListener(EventType.MENU_MATCHMAKING_ClientMatchmakingCancelled, CancelMatchmaking);
    }

    /// <summary>
    /// On a client disconnects (includes other players), invoke MENU_OnClientPlayerDisconnected event
    /// This is invoked by the server when a player on the server disconnects
    /// </summary>
    [ClientRpc]
    public void RpcOnServerPlayerStop(PlayerTeamInfo info) {
        //print($"{info.DisplayName} left the server, teamID: {info.teamId}");
        //EventCenter.Broadcast(EventType.MENU_OnClientPlayerDisconnected, teamInfo);
    }

    /// <summary>
    /// On a client disconnects (includes other players), invoke MENU_OnClientPlayerDisconnected event
    /// This is invoked by the server when a player on the server disconnects
    /// </summary>
   /* public override void OnStopClient() {
        print($"{teamInfo.DisplayName} left the server, teamID: {teamInfo.teamId}");
        EventCenter.Broadcast(EventType.MENU_OnClientPlayerDisconnected, teamInfo);
    }*/

    [Client]
    public void RequestMatch(Mode gamemode) {
        if (hasAuthority) {
            ticketId = "";
            EventCenter.Broadcast(EventType.MENU_MATCHMAKING_ClientRequestingMatchmaking, true, true,
                "MANU_WAITING_MATCHMAKING");
            RunServerCommand<Mode>(CmdServerRequestMatch, gamemode);
        }
    }


    [TargetRpc]
    private void TargetStartPlayFabMatchmaking(NetworkConnection target) {
        ticketId = "";
        StartCoroutine(StartPlayFabMatchmaking(target));
    }

    private IEnumerator StartPlayFabMatchmaking(NetworkConnection target) {
        pollTicketCancelled = false;
        yield return new WaitForSeconds(0.5f);
        print($"[MasterServerPlayer - {teamInfo.DisplayName}]: Requesting PlayFab matchmaking, queue name: {GameMode.GetGameModeObj(requestingMode).GetQueueName()}" +
              $"Entity id: {entityId}");
        
        PlayFabMultiplayerAPI.CreateMatchmakingTicket(new CreateMatchmakingTicketRequest
        {
            Creator = new MatchmakingPlayer
            {
                Entity = new EntityKey
                {
                    Id = entityId,
                    Type = "title_player_account"
                },
                Attributes = new MatchmakingPlayerAttributes
                {
                    DataObject = new { }
                }
            },
            GiveUpAfterSeconds = 120,
            QueueName = GameMode.GetGameModeObj(requestingMode).GetQueueName(),
        }, ClientOnMatchmakingTicketCreated, ClientOnMatchmakingError);
    }


    private Coroutine pollTicketCoroutine;

    private string ticketId;

    [Client]
    private void ClientOnMatchmakingTicketCreated(CreateMatchmakingTicketResult result) {
        print($"[MasterServerPlayer - {teamInfo.DisplayName}]: Client matchmaking ticket created");
        this.ticketId = result.TicketId;
        pollTicketCoroutine = StartCoroutine(ClientPollTicket());
    }

    private bool pollTicketCancelled = false;
    [Client]
    private IEnumerator ClientPollTicket()
    {
        
        print($"[MasterServerPlayer - {teamInfo.DisplayName}]: polling ticket, ticket id: " + this.ticketId);

        while (!pollTicketCancelled)
        {
            PlayFabMultiplayerAPI.GetMatchmakingTicket(
                new GetMatchmakingTicketRequest
                {
                    TicketId = this.ticketId,
                    QueueName = GameMode.GetGameModeObj(requestingMode).GetQueueName()
                }, OnGetMatchmakingTicket, ClientOnMatchmakingError);
            yield return new WaitForSeconds(6);
        }
    }

    [Client]
    private void OnGetMatchmakingTicket(GetMatchmakingTicketResult result)
    {
        if (!pollTicketCancelled) {
            print($"[MasterServerPlayer - {teamInfo.DisplayName}]: Polling a ticket. Result: " + result.Status);
            //WaitingForPlayers, WaitingForMatch, WaitingForServer, Canceled, Matched
            switch (result.Status)
            {
                case "Matched":
                    StopCoroutine(pollTicketCoroutine);
                    ClientStartMatch(result.MatchId);
                    break;
                case "Canceled":
                    print($"[MasterServerPlayer - {teamInfo.DisplayName}]: Cancelled");
                    StopCoroutine(pollTicketCoroutine);
                    break;
            }
        }
        else
        {
            CancelMatchmaking();
        }

    }

    [Client]
    private void ClientStartMatch(string matchId)
    {
        if (hasAuthority) {
            EventCenter.Broadcast(EventType.MENU_MATCHMAKING_ClientMatchmakingReadyToGet);
            PlayFabMultiplayerAPI.GetMatch(
                new GetMatchRequest
                {
                    MatchId = matchId,
                    QueueName = GameMode.GetGameModeObj(requestingMode).GetQueueName()
                },
                CmdOnGetMatch,
                ClientOnMatchmakingError
            );
        }
    }

    [TargetRpc]
    private void TargetOnServerGetMatch(PlayerTeamInfo thisTeamInfo) {
        EventCenter.Broadcast(EventType.MENU_MATCHMAKING_ClientMatchmakingSuccess, thisTeamInfo);
    }

    [TargetRpc]
    private void TargetOnServerFailedToGetMatch()
    {
        EventCenter.Broadcast(EventType.MENU_MATCHMAKING_ClientMatchmakingFailed);
        CancelMatchmaking();
        
    }

    [Client]
    private void ClientOnMatchmakingError(PlayFabError error) {


        EventCenter.Broadcast(EventType.MENU_MATCHMAKING_ClientMatchmakingFailed);
        if (pollTicketCoroutine != null) {
            StopCoroutine(pollTicketCoroutine);
            pollTicketCoroutine = null;
        }
        
        CancelMatchmaking();
    }


    [TargetRpc]
    private void TargetOnLobbyInfoUpdated(PlayerTeamInfo[] infos, PlayerTeamInfo myInfo) {
        if (hasAuthority) {
            EventCenter.Broadcast(EventType.MENU_OnClientLobbyInfoUpdated,infos,myInfo);
        }
    }


    [Client]
    private void CancelMatchmaking() {
        pollTicketCancelled = true;
        if (hasAuthority && ticketId!="") {
            PlayFabMultiplayerAPI.CancelAllMatchmakingTicketsForPlayer(new CancelAllMatchmakingTicketsForPlayerRequest {
                QueueName = GameMode.GetGameModeObj(requestingMode).GetQueueName()
            },ClientOnTicketCanceled, (error) => { print(error.Error);});
            
        }
    }

    [Client]
    private void ClientOnTicketCanceled(CancelAllMatchmakingTicketsForPlayerResult result) {

        if (pollTicketCoroutine != null) {
            StopCoroutine(pollTicketCoroutine);
            pollTicketCoroutine = null;
        }

        pollTicketCancelled = true;
    }

    [TargetRpc]
    private void TargetOnLobbyStateUpdated(MatchState state, string ip,ushort port) {
        if (hasAuthority) {
            EventCenter.Broadcast(EventType.MENU_OnClientLobbyStateUpdated, state,ip,port,requestingMode);
        }
    }

    /// <summary>
    /// Request the server to leave the current lobby
    /// </summary>
    [Client]
    public void ClientRequestLeaveLobby() {
        RunServerCommand(CmdRequestLeaveLobby);
    }

    [TargetRpc]
    private void TargetOnLeaveLobbyFailed(MatchError error) {
        EventCenter.Broadcast(EventType.MENU_OnClientLeaveLobbyFailed,error);
        CancelMatchmaking();
    }

    [TargetRpc]
    private void TargetOnLeaveLobbySuccess() {
        EventCenter.Broadcast(EventType.MENU_OnClientLeaveLobbySuccess);
        CancelMatchmaking();
    }

    [TargetRpc]
    private void TargetOnMatchReadyToGet() {
        EventCenter.Broadcast(EventType.MENU_MATCHMAKING_ClientMatchmakingReadyToGet);
    }

    [TargetRpc]
    private void TargetUpdateCountDown(float countdown) {
        EventCenter.Broadcast(EventType.MENU_OnClientLobbyCountdownUpdated,countdown);
    }

    [TargetRpc]
    private void TargetKicked() {
        EventCenter.Broadcast(EventType.MENU_OnAuthenticaeteFailed);
        NetworkManager.singleton.StopClient();
    }


    /*
    //for future: add avatar
    [TargetRpc]
    private void TargetOnNewPlayerJoinsLobby(PlayerTeamInfo teamInfo) {
        Debug.Log($"{displayName} joined the lobby");
        if (hasAuthority) {
            EventCenter.Broadcast(EventType.MENU_OnClientNewPlayerJoinLobby, teamInfo);
        }
    }*/



    #endregion

}
