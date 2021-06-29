using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using UnityEngine;


public class GameBaseNetworkManager : NetworkManager {
    private string ip;
    private ushort port;
    private string matchId;

    [SerializeField] private Mode gamemode;

    private List<GamePlayer> existingPlayers;
    private List<PlayerTeamInfo> matchPlayerInfos;

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


    #region Server
    [ServerCallback]
    public override void OnStartServer() {
        base.OnStartServer();
        existingPlayers = new List<GamePlayer>();
        EventCenter.AddListener<GamePlayer>(EventType.GAME_ServerOnPlayerConnected, AuthenticatePlayer);
        EventCenter.AddListener<GamePlayer>(EventType.GAME_ServerOnPlayerDisconnected, RemovePlayer);
    }

    public override void OnStopServer() {
        base.OnStopServer();
        EventCenter.RemoveListener<GamePlayer>(EventType.GAME_ServerOnPlayerConnected, AuthenticatePlayer);
        EventCenter.RemoveListener<GamePlayer>(EventType.GAME_ServerOnPlayerDisconnected, RemovePlayer);
    }

    [ServerCallback]
    private void RemovePlayer(GamePlayer player) {
        PlayerTeamInfo playerTeamInfo = FindPlayerTeamInfo(player);
        if (playerTeamInfo != null) {
            matchPlayerInfos.Remove(playerTeamInfo);
            Debug.Log($"Removed player {player.Username} from the game");
        }
    }

    [ServerCallback]
    private PlayerTeamInfo FindPlayerTeamInfo(GamePlayer player) {
        foreach (PlayerTeamInfo playerInfo in matchPlayerInfos) {
            if (playerInfo.username == player.Username) {
                return playerInfo;
            }
        }

        return null;
    }

    [ServerCallback]
    private void AuthenticatePlayer(GamePlayer player) {
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
    [Client]
    private void OnJoinGameServerFailed()
    {
        Debug.Log("Join server failed");
        EventCenter.Broadcast(EventType.GAME_OnClientConnectingToServerFailed);
    }

    [Client]
    private void OnJoiningGameServer()
    {
        EventCenter.Broadcast(EventType.GAME_OnClientConnectingToServer);
    }

    #endregion


}


