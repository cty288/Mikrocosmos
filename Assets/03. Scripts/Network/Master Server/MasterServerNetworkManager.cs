using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MasterServerNetworkManager : NetworkManager {
    private List<MasterServerPlayer> players;

    private MatchManager matchManager;
    public MatchManager MatchManager => matchManager;

    #region Server
    [Server]
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        //players.Add(conn.identity.GetComponent<MasterServerPlayer>());
        StartCoroutine(Wait(conn));
    }

    IEnumerator Wait(NetworkConnection conn) {
        yield return new WaitForSeconds(0.3f);
        players.Add(conn.identity.GetComponent<MasterServerPlayer>());
    }

    [Server]
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        players.Remove(conn.identity.GetComponent<MasterServerPlayer>());
    }


    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server start!");
        InitializeServerOnlyObjs();
    }

    /// <summary>
    /// Called in OnStartServer(). Add server only object to the server
    /// </summary>
    [Server]
    private void InitializeServerOnlyObjs() {
        matchManager= MirrorServerUtilities.SpawnServerOnlyObject<MatchManager>("Match Manager").GetComponent<MatchManager>();
    }

    
    public GameMatch ServerRequestFindAvailableMatch(Mode gamemode) {
        if (matchManager && NetworkServer.active) {
            GameMatch match= matchManager.FindAvailableMatch(GameMode.GetGameModeObj(gamemode));
            Debug.Log("Find the MatchManager");
            if (match != null) {
                return match;
            }
        }
        Debug.Log("Unable to find a match");
        return null;
    }
    #endregion


    #region Client


    #endregion
}
