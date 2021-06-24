using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using PlayFab;
using PlayFab.MultiplayerModels;
using UnityEngine;
using UnityEngine.Events;

public class MasterServerPlayer : NetworkBehaviour {
    [SyncVar] 
    private long timeIntervalPerCommand = 300;

    [SyncVar] private long lastCommandTick = 0;

    [SyncVar][SerializeField]
    private string matchId;

    [SyncVar][SerializeField]
    private string displayName;
    public string DisplayName => displayName;

    [SyncVar]
    [SerializeField]
    private string entityId;
    

    [SyncVar] 
    private Mode requestingMode; 

    #region Server
    public override void OnStartServer() {
        EventCenter.Broadcast(EventType.MENU_OnServerPlayerAdded,this);
    }

    public Action<MasterServerPlayer> onPlayerDisconnect;
    public override void OnStopServer() {
        onPlayerDisconnect?.Invoke(this);
        EventCenter.Broadcast(EventType.MENU_OnServerPlayerDisconnected,this);
    }

    [Command]
    private void CmdUpdateLastCommandTick() {
        lastCommandTick = DateTime.Now.Ticks;
    }

    [Command]
    private void CmdServerRequestMatch(Mode mode) {
        GameMatch match = ((MasterServerNetworkManager) NetworkManager.singleton).ServerRequestFindAvailableMatch(mode);
        requestingMode = mode;
        if (match != null) {
            matchId = match.MatchId;
            if (match.JoinPlayer(this)) {
                TargetOnServerGetMatch(matchId);
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

    [Command]
    private void CmdOnGetMatch(GetMatchResult result)
    {
        GameMatch match =
            ((MasterServerNetworkManager) NetworkManager.singleton).ServerRequestNewPlayfabMatchmakingRoom(
                GameMode.GetGameModeObj(requestingMode), result.MatchId);
        
        if (match != null) {
            matchId = match.MatchId;
            if (match.JoinPlayer(this)) {
                TargetOnServerGetMatch(matchId);
            }
            else {
                TargetOnServerFailedToGetMatch();
            }
            
        }
        else {
            TargetOnServerFailedToGetMatch();
        }
    }
    
    

    [Command]
    private void CmdUpdatePlayfabToken(PlayfabToken token) {
        this.displayName = token.PlayerName;
        this.entityId = token.EntityId;
        print(displayName);
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
        base.OnStartAuthority();
        EventCenter.Broadcast(EventType.MENU_AuthorityOnConnected);
        EventCenter.AddListener(EventType.MENU_MATCHMAKING_ClientMatchmakingCancelled,CancelMatchmaking);
        if (PlayfabTokenPasser._instance) {
            CmdUpdatePlayfabToken(PlayfabTokenPasser._instance.Token);
        }
    }

    public override void OnStopAuthority() {
        base.OnStopAuthority();
        EventCenter.RemoveListener(EventType.MENU_MATCHMAKING_ClientMatchmakingCancelled, CancelMatchmaking);
    }

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
        yield return new WaitForSeconds(0.5f);
        print($"Requesting PlayFab matchmaking, queue name: {GameMode.GetGameModeObj(requestingMode).GetQueueName()}" +
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
        print("Client matchmaking ticket created");
        this.ticketId = result.TicketId;
        pollTicketCoroutine = StartCoroutine(ClientPollTicket());
    }

    [Client]
    private IEnumerator ClientPollTicket()
    {
        
        print("polling ticket, ticket id: "+this.ticketId);
        while (true)
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
        print("Polling a ticket. Result: "+result.Status);
        //WaitingForPlayers, WaitingForMatch, WaitingForServer, Canceled, Matched
        switch (result.Status)
        {
            case "Matched":
                StopCoroutine(pollTicketCoroutine);
                ClientStartMatch(result.MatchId);
                break;
            case "Canceled":
                print("Cancelled");
                StopCoroutine(pollTicketCoroutine);
                break;
        }
    }

    [Client]
    private void ClientStartMatch(string matchId)
    {
        if (hasAuthority) {
            print("Starting a match.");
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
    private void TargetOnServerGetMatch(string matchId) {
        EventCenter.Broadcast(EventType.MENU_MATCHMAKING_ClientMatchmakingSuccess, matchId);
    }

    [TargetRpc]
    private void TargetOnServerFailedToGetMatch()
    {
        EventCenter.Broadcast(EventType.MENU_MATCHMAKING_ClientMatchmakingFailed);
        CancelMatchmaking();
    }

    [Client]
    private void ClientOnMatchmakingError(PlayFabError error) {
        print("Client matchmaking error occurred: "+error.Error.ToString());
        EventCenter.Broadcast(EventType.MENU_MATCHMAKING_ClientMatchmakingFailed);
        if (pollTicketCoroutine != null) {
            StopCoroutine(pollTicketCoroutine);
        }

        CancelMatchmaking();
    }

    [Client]
    private void CancelMatchmaking() {
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
        }
        print("Matchmaking ticket cancelled success");
    }

    #endregion

}
