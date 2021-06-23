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

    [SyncVar] 
    private PlayfabToken playfabToken;

    #region Server
    [Command]
    private void CmdUpdateLastCommandTick() {
        lastCommandTick = DateTime.Now.Ticks;
    }

    [Command]
    private void CmdServerRequestMatch(Mode mode) {
        GameMatch match = ((MasterServerNetworkManager) NetworkManager.singleton).ServerRequestFindAvailableMatch(mode);
        if (match != null) {
            matchId = match.MatchId;
        }
        else {
            //Playfab start matchmaking. This need to be called on the Client due to PlayFab Rate Limit
            TargetStartPlayFabMatchmaking(connectionToClient,mode);
        }
    }

    
    [Command]
    private void CmdUpdatePlayfabToken(PlayfabToken token) {
        this.playfabToken = new PlayfabToken(token.SessionTicket,token.EntityId,token.PlayfabId,token.PlayerName);
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
        if (PlayfabTokenPasser._instance) {
            CmdUpdatePlayfabToken(PlayfabTokenPasser._instance.Token);
        }
    }

    [Client]
    public void RequestMatch(Mode gamemode) {
        if (hasAuthority) {
            RunServerCommand<Mode>(CmdServerRequestMatch, gamemode);
        }
    }

    public void Test(GameMatch match) {
        print(match.Ip);
    }

    [TargetRpc]
    private void TargetStartPlayFabMatchmaking(NetworkConnection target,Mode mode) {
        PlayFabMultiplayerAPI.CreateMatchmakingTicket(new CreateMatchmakingTicketRequest
        {
            Creator = new MatchmakingPlayer
            {
                Entity = new EntityKey
                {
                    Id = playfabToken.EntityId, 
                    Type = "title_player_account"
                },
                Attributes = new MatchmakingPlayerAttributes
                {
                    DataObject = new { }
                }
            },
            GiveUpAfterSeconds = 120,
            QueueName = GameMode.GetGameModeObj(mode).GetQueueName(),
        }, ClientOnMatchmakingTicketCreated, ClientOnMatchmakingError);
        print("Queue Name: "+GameMode.GetGameModeObj(mode).GetQueueName());
    }

    [Client]
    private void ClientOnMatchmakingTicketCreated(CreateMatchmakingTicketResult result) {
        print("Client matchmaking ticket created");
    }

    [Client]
    private void ClientOnMatchmakingError(PlayFabError error) {
        print("Client matchmaking error occurred: "+error.Error.ToString());
    }

    #endregion

}
