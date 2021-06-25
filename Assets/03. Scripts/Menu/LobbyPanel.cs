using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour {
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private Transform[] teams;
    [SerializeField] private GameObject gamestartInfo;
    [SerializeField] private Button leaveLobbyButton;
    [Tooltip("Each prefab corresponds to each team in the teams array")]
    [SerializeField] private GameObject[] playerInfoPrefabs;

    void Awake() {
        EventCenter.AddListener<string,int,PlayerTeamInfo[]>(EventType.MENU_MATCHMAKING_ClientMatchmakingSuccess,
            HandleClientJoinLobby);
        EventCenter.AddListener<string,int>(EventType.MENU_OnClientNewPlayerJoinLobby,HandleOnNewPlayerJoinLobby);
        EventCenter.AddListener<string,int>(EventType.MENU_OnClientPlayerDisconnected,HandleOnPlayerDisconnected);
    }

    void OnDestroy() {
        EventCenter.RemoveListener<string, int>(EventType.MENU_OnClientNewPlayerJoinLobby, HandleOnNewPlayerJoinLobby);
        EventCenter.RemoveListener<string, int, PlayerTeamInfo[]>(EventType.MENU_MATCHMAKING_ClientMatchmakingSuccess,
            HandleClientJoinLobby);
        EventCenter.RemoveListener<string,int>(EventType.MENU_OnClientPlayerDisconnected, HandleOnPlayerDisconnected);
    }


    private void HandleClientJoinLobby(string matchId, int teamId,PlayerTeamInfo[] infos) {
        if (lobbyPanel) {
            lobbyPanel.SetActive(true);
        }

        string clientName = PlayfabTokenPasser._instance.Token.PlayerName;
        DisplayPlayerToLobby(clientName,teamId);
       
        for (int i = 0; i < infos.Length; i++)
        {
            if (infos[i].DisplayName != clientName && !String.IsNullOrEmpty(infos[i].DisplayName))
            {
                DisplayPlayerToLobby(infos[i].DisplayName,infos[i].teamId);
            }
        }
    }

    private void HandleOnNewPlayerJoinLobby(string displayName, int teamId) {
        print($"New Player {displayName} Join team {teamId}");
        DisplayPlayerToLobby(displayName, teamId);
    }

    /// <summary>
    /// When a player (including other players) disconnected, remove them from the lobby player list
    /// </summary>
    /// <param name="player"></param>
    private void HandleOnPlayerDisconnected(string playerName,int teamId) {
        if (teamId < 0) {
            return;
        }

       
        if (teams[teamId]) {
            Transform team = teams[teamId];
            PlayerInfo[] playerInfos = team.GetComponentsInChildren<PlayerInfo>();
            if (lobbyPanel.activeInHierarchy)
            {
                if (playerInfos != null && playerInfos.Length > 0)
                {
                    foreach (PlayerInfo playerInfo in playerInfos)
                    {
                        if (playerInfo != null)
                        {
                            if (playerInfo.GetDisplayName() == playerName)
                            {
                                Destroy(playerInfo.gameObject);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }


    private void DisplayPlayerToLobby(string displayName, int teamId) {
        if (teamId >= 0) {
            GameObject playerInfoObj = Instantiate(playerInfoPrefabs[teamId]);
            playerInfoObj.transform.parent = teams[teamId];

            PlayerInfo playerInfo = playerInfoObj.GetComponent<PlayerInfo>();
            playerInfo.SetDisplayName(displayName);
        }

    }
}
