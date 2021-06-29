using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GamePlayer : NetworkBehaviour {
    private string username;
    public string Username => username;

    [SyncVar(hook="OnAuthenticated")] 
    private int authenticateSuccess = -1; //-1; unauthenticated, 0 false, 1 true

    #region Server
    [Command]
    private void CmdUpdatePlayfabToken(PlayfabToken token) {
        this.username = token.Username;
        EventCenter.Broadcast(EventType.GAME_ServerOnPlayerConnected, this);
    }

    [ServerCallback]
    public void Authenticate(int isSuccess) {
        authenticateSuccess = isSuccess;
    }

    public override void OnStopServer() {
        EventCenter.Broadcast(EventType.GAME_ServerOnPlayerDisconnected, this);
    }

    #endregion


    #region Client
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        EventCenter.Broadcast(EventType.GAME_OnClientConnectingToServerSuccess);
        if (PlayfabTokenPasser._instance) {
            CmdUpdatePlayfabToken(PlayfabTokenPasser._instance.Token);
        }
    }

    private void OnAuthenticated(int oldValue, int newValue) {
        if (hasAuthority) {
            if (newValue == 1) {
                Debug.Log("Authenticate success!");
                EventCenter.Broadcast(EventType.GAME_OnClientAuthenticated,true);
            }else
            {
                Debug.Log("Authenticate failed!");
                EventCenter.Broadcast(EventType.GAME_OnClientAuthenticated,false);
            }
        }

    }
    #endregion

}
