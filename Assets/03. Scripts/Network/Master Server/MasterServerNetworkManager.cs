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
        players.Remove(conn.identity.GetComponent<MasterServerPlayer>());
        base.OnServerDisconnect(conn);
        
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
    /// <summary>
    /// Request MatchManager to create a new match room based on generated playfab matchid and gamemode
    /// </summary>
    /// <param name="gamemode"></param>
    /// <param name="matchId"></param>
    /// <returns></returns>
    public GameMatch ServerRequestNewPlayfabMatchmakingRoom(GameMode gamemode, string matchId) {
        if (matchManager && NetworkServer.active) {
            return matchManager.RequestNewPlayfabMatchmakingRoom(gamemode, matchId);
        }

        return null;
    }
    #endregion


    #region Client


    #endregion
}
