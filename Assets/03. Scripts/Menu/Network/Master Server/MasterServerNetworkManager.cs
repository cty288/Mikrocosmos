using System;
using System.Collections;
using System.Collections.Generic;

using MikroFramework.Event;
using Mirror;
using UnityEngine;
using EventType = MikroFramework.Event.EventType;

public class MasterServerNetworkManager : NetworkManager {


    private List<MasterServerPlayer> playersConnections;

    private MatchManager matchManager;
    public MatchManager MatchManager => matchManager;


    #region Server
    [Server]
    public void AddPlayer(MikroMessage msg) {
        MasterServerPlayer player = msg.GetSingleMessage() as MasterServerPlayer;
        
        print($"[MasterServerNetworkManager] Added {player.TeamInfo.DisplayName} to the server player list");
        playersConnections.Add(player);

    }

    [Server]
    public void RemovePlayer(MikroMessage msg)
    {
        MasterServerPlayer player = msg.GetSingleMessage() as MasterServerPlayer;

        if (player) {
            print($"[MasterServerNetworkManager] Removed {player.TeamInfo.DisplayName} from the server player list");
            playersConnections.Remove(player);
        }

    }
    
    /*
    [Server]
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        print($"Removed {conn.identity.GetComponent<MasterServerPlayer>().DisplayName} from the server player list");
        playersConnections.Remove(conn.identity.GetComponent<MasterServerPlayer>());
        base.OnServerDisconnect(conn);
        
    }*/


    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        playersConnections = new List<MasterServerPlayer>();
        Debug.Log("[MasterServerNetworkManager] Server start!");
        InitializeServerOnlyObjs();
        AddListener(EventType.MENU_OnServerPlayerAdded,AddPlayer);
        AddListener(EventType.MENU_OnServerPlayerDisconnected, RemovePlayer);
    }

    [Server]
    public override void OnStopServer() {
        base.OnStopServer();
        RemoveListener(EventType.MENU_OnServerPlayerAdded, AddPlayer);
        RemoveListener(EventType.MENU_OnServerPlayerDisconnected, RemovePlayer);
    }

    /// <summary>
    /// Called in OnStartServer(). Add server only object to the server
    /// </summary>
    [Server]
    private void InitializeServerOnlyObjs() {
        matchManager= MirrorServerUtilities.SpawnServerOnlyObject<MatchManager>("Match Manager").GetComponent<MatchManager>();
        MirrorServerUtilities.SpawnServerOnlyObject<MenuServerDatabaseManager>("Menu Database Manager");
    }

    
    public GameMatch ServerRequestFindAvailableMatch(Mode gamemode) {
        if (matchManager && NetworkServer.active) {
            GameMatch match= matchManager.FindAvailableMatch(GameMode.GetGameModeObj(gamemode));
            if (match != null) {
                return match;
            }
        }
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
