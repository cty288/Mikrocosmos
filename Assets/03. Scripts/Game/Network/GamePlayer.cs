using System.Collections;
using System.Collections.Generic;
using MikroFramework.Event;
using Mirror;
using UnityEngine;
using EventType = MikroFramework.Event.EventType;

public class GamePlayer : NetworkBehaviour {
    private string username;
    public string Username => username;

    [SyncVar(hook="OnAuthenticated")] 
    private int authenticateSuccess = -1; //-1; unauthenticated, 0 false, 1 true

    #region Server
    [Command]
    private void CmdUpdatePlayfabToken(PlayfabToken token) {
        this.username = token.Username;
        Broadcast(EventType.GAME_ServerOnPlayerConnected, MikroMessage.Create(this));
    }

    [ServerCallback]
    public void Authenticate(int isSuccess) {
        authenticateSuccess = isSuccess;
    }

    public override void OnStopServer() {
       Broadcast(EventType.GAME_ServerOnPlayerDisconnected, MikroMessage.Create(this));
    }

    #endregion


    #region Client
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        Broadcast(EventType.GAME_OnClientConnectingToServerSuccess,null);
        if (PlayfabTokenPasser._instance) {
            CmdUpdatePlayfabToken(PlayfabTokenPasser._instance.Token);
        }
    }

    private void OnAuthenticated(int oldValue, int newValue) {
        if (hasAuthority) {
            if (newValue == 1) {
                Debug.Log("Authenticate success!");
                Broadcast(EventType.GAME_OnClientAuthenticated,MikroMessage.Create(true));
            }else
            {
                Debug.Log("Authenticate failed!");
                Broadcast(EventType.GAME_OnClientAuthenticated,MikroMessage.Create(false));
            }
        }

    }
    #endregion

}
