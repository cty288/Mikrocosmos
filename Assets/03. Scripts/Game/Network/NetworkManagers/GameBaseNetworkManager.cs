using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Event;
using Mirror;
using UnityEngine;
using EventType = MikroFramework.Event.EventType;


public class GameBaseNetworkManager : NetworkManager {
    private string ip;
    private ushort port;
    private string matchId;

    [SerializeField] private Mode gamemode;

    private List<GamePlayer> existingPlayers;
    private List<PlayerTeamInfo> matchPlayerInfos;

    [Tooltip("The game process will periodically check the number of players. " +
             "It will self-destroy if there's no players in the game. ")]
    [SerializeField] private float inactiveSelfDestroyTime = 30f;

    private void ServerSetup() {
#if UNITY_SERVER
            string[] args = Environment.GetCommandLineArgs();
            print($"ip: {args[1]}, port: {args[2]}, matchId: {args[3]}");
            ip = args[1];
            ushort.TryParse(args[2],out port);
            matchId = args[3];
            int totalPlayer = int.Parse(args[4]);

            matchPlayerInfos = new List<PlayerTeamInfo>();

            List<string> playerUsernamesInGame = new List<string>();
            List<int> playerTeamIds = new List<int>();
            List<string> playerDisplayNames = new List<string>();

            if (!string.IsNullOrEmpty(ip) && port >= 7778 && !string.IsNullOrEmpty(matchId)) {
                
                for (int i = 5; i < 5+totalPlayer; i++) {
                    playerUsernamesInGame.Add(args[i]);
                }

                for (int i = 5 + totalPlayer; i < 5 + totalPlayer * 2; i++) {
                    playerTeamIds.Add(int.Parse(args[i]));
                }

                for (int i = 5 + totalPlayer * 2; i < 5 + totalPlayer * 3; i++) {
                    playerDisplayNames.Add(args[i]);
                }

                for (int i = 0; i < totalPlayer; i++) {
                    string displayName = playerDisplayNames[i];
                    string username = playerUsernamesInGame[i];
                    int teamId = playerTeamIds[i];
                    matchPlayerInfos.Add(new PlayerTeamInfo(displayName,teamId,matchId,username));
                    Debug.Log($"Added {username} to the game. Display name {displayName}. TeamId: {teamId}");
                }

                NetworkManager.singleton.networkAddress = ip;
                NetworkManager.singleton.GetComponent<TelepathyTransport>().port = port;
                NetworkManager.singleton.StartServer();
                Debug.Log($"{matchId} game process successfully spawned!");
            }
            else {
                Debug.Log($"Missing arguments! {matchId} auto close on the server!");
                Application.Quit();
            }
#else
        NetworkConnector._singleton.ConnectToServer(PlayerPrefs.GetString("ip"),
                 (ushort)PlayerPrefs.GetInt("port"), OnJoiningGameServer, OnJoinGameServerFailed, 1.5f, 2f, 60);
#endif
    }
    
    void Start()
    {
        ServerSetup();
    }

    public int GetExistingPlayerNumber()
    {
        return NetworkManager.singleton.numPlayers;
    }
    #region Server
    /// <summary>
    /// Add a bunch of listeners...
    /// </summary>
    [ServerCallback]
    public override void OnStartServer() {
        base.OnStartServer();
        existingPlayers = new List<GamePlayer>();
        AddListener(EventType.GAME_ServerOnPlayerConnected, AuthenticatePlayer);
        AddListener(EventType.GAME_ServerOnPlayerDisconnected, RemovePlayer);
        StartCoroutine(StartMonitoringPlayerNum());
    }
    /// <summary>
    /// remove listeners when the server stops
    /// </summary>
    public override void OnStopServer() {
        base.OnStopServer();
        RemoveListener(EventType.GAME_ServerOnPlayerConnected, AuthenticatePlayer);
        RemoveListener(EventType.GAME_ServerOnPlayerDisconnected, RemovePlayer);
    }

    [ServerCallback]
    private IEnumerator StartMonitoringPlayerNum()
    {
        yield return new WaitForSeconds(60);
        StartCoroutine(SelfDestroyAfterInactive());
    }

    [ServerCallback]
    private IEnumerator SelfDestroyAfterInactive()
    {
        while (true)
        {
            yield return new WaitForSeconds(inactiveSelfDestroyTime);
            if (GetExistingPlayerNumber() <= 0)
            {
                NetworkManager.singleton.StopServer();
                Debug.Log($"Match {matchId} is self-destroyed because all players have left the room");
                Application.Quit();
            }
        }
    }


    [ServerCallback]
    private void RemovePlayer(MikroMessage msg) {
        GamePlayer player = msg.GetSingleMessage() as GamePlayer;
        
        //check if the player exists in the matchPlayerInfos first
        PlayerTeamInfo playerTeamInfo = FindPlayerTeamInfo(player);
        if (playerTeamInfo != null) {
            matchPlayerInfos.Remove(playerTeamInfo);
            Debug.Log($"Removed player {player.Username} from the game");
        }
    }

    /// <summary>
    /// Find the player's corresponding PlayerTeamInfo object based on GamePlayer object
    /// It will return something only if the player has been authenticated
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    [ServerCallback]
    private PlayerTeamInfo FindPlayerTeamInfo(GamePlayer player) {
        foreach (PlayerTeamInfo playerInfo in matchPlayerInfos) {
            if (playerInfo.username == player.Username) {
                return playerInfo;
            }
        }

        return null;
    }
    /// <summary>
    /// Authenticate the player. Make sure the joined player has a record in "matchPlayerInfos"
    /// </summary>
    /// <param name="player"></param>
    [ServerCallback]
    private void AuthenticatePlayer(MikroMessage msg) {
        GamePlayer player = msg.GetSingleMessage() as GamePlayer;
        
        PlayerTeamInfo playerInfo = FindPlayerTeamInfo(player);

        if (playerInfo!=null) {
            Debug.Log($"{player.Username} exists in this match. Authenticate pass");
            existingPlayers.Add(player);
            player.Authenticate(1);
        }
        else {
            Debug.Log($"{player.Username} doesn't exist in this match! Kicking it back to the menu...");
            ClearPlayerMatchidInfo();
            player.Authenticate(0);
        }
    }
    /// <summary>
    /// Clear the player's match id info on the database
    /// </summary>
    private void ClearPlayerMatchidInfo() {

    }
    #endregion


    #region Client

    private void OnJoinGameServerFailed(MikroMessage msg)
    {
        Debug.Log("Join server failed");
        Broadcast(EventType.GAME_OnClientConnectingToServerFailed,null);
    }

    [Client]
    private void OnJoiningGameServer(MikroMessage msg)
    {
        Broadcast(EventType.GAME_OnClientConnectingToServer,null);
    }

    #endregion


}


