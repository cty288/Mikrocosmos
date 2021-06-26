using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour {
    [SerializeField] private LobbyPanelChild lobbyPanel;

    [SerializeField] private PlayerInfo[] team1Info;
    [SerializeField] private PlayerInfo[] team2Info;

    [SerializeField] private GameObject gamestartInfo;
    [SerializeField] private Button leaveLobbyButton;
    [Tooltip("Each prefab corresponds to each team in the teams array")]
    [SerializeField] private GameObject[] playerInfoPrefabs;

    void Awake() {
        
        EventCenter.AddListener<PlayerTeamInfo>(EventType.MENU_MATCHMAKING_ClientMatchmakingSuccess,
            HandleClientJoinLobby);
        LobbyPanelChild.onLobbyPanelChildEnabled += AddLobbyPanelChildListeners;
        LobbyPanelChild.onLobbyPanelChildDisabled += RemoveLobbyPanelChildListeners;
        // EventCenter.AddListener<PlayerTeamInfo>(EventType.MENU_OnClientNewPlayerJoinLobby,HandleOnNewPlayerJoinLobby);
        // EventCenter.AddListener<PlayerTeamInfo>(EventType.MENU_OnClientPlayerDisconnected,HandleOnPlayerDisconnected);
    }

    void OnDestroy() {

        EventCenter.RemoveListener<PlayerTeamInfo>(EventType.MENU_MATCHMAKING_ClientMatchmakingSuccess,
            HandleClientJoinLobby);
        LobbyPanelChild.onLobbyPanelChildEnabled -= AddLobbyPanelChildListeners;
        LobbyPanelChild.onLobbyPanelChildDisabled -= RemoveLobbyPanelChildListeners;
        // EventCenter.RemoveListener<PlayerTeamInfo>(EventType.MENU_OnClientNewPlayerJoinLobby, HandleOnNewPlayerJoinLobby);
        // EventCenter.RemoveListener<PlayerTeamInfo>(EventType.MENU_OnClientPlayerDisconnected, HandleOnPlayerDisconnected);
    }


    private void AddLobbyPanelChildListeners() {
        EventCenter.AddListener<PlayerTeamInfo[]>(EventType.MENU_OnClientLobbyInfoUpdated,DisplayPlayerToLobby);
    }

    private void RemoveLobbyPanelChildListeners() {
        EventCenter.RemoveListener<PlayerTeamInfo[]>(EventType.MENU_OnClientLobbyInfoUpdated, DisplayPlayerToLobby);
    }

    private void HandleClientJoinLobby(PlayerTeamInfo thisPlayerInfo) {
        if (lobbyPanel) {
            lobbyPanel.gameObject.SetActive(true);
            ClearClientLobby();
        }
    }

    /*
    private void HandleOnNewPlayerJoinLobby(PlayerTeamInfo info) {
        print($"New Player {info.DisplayName} Join team {info.teamId}");
        DisplayPlayerToLobby(info);
    }

    /// <summary>
    /// When a player (including other players) disconnected, remove them from the lobby player list
    /// </summary>
    /// <param name="player"></param>
    private void HandleOnPlayerDisconnected(PlayerTeamInfo teamInfo){
        if (lobbyPanel.gameObject.activeInHierarchy) {
            Debug.Log($"Lobby Panel: Detected {teamInfo.DisplayName} left the server. TeamId: {teamInfo.teamId}");
            if (teamInfo.teamId < 0)
            {
                bool success = false;
                for (int i = 0; i < teams.Length; i++)
                {
                    success = HandleOnPlayerDisconnected(i, teamInfo.DisplayName);
                    if (success)
                    {
                        return;
                    }
                }

                return;
            }


            if (teams[teamInfo.teamId])
            {
                Transform team = teams[teamInfo.teamId];
                PlayerInfo[] playerInfos = team.GetComponentsInChildren<PlayerInfo>();
                if (lobbyPanel.gameObject.activeInHierarchy)
                {
                    if (playerInfos != null && playerInfos.Length > 0)
                    {
                        foreach (PlayerInfo playerInfo in playerInfos)
                        {
                            if (playerInfo != null)
                            {
                                if (playerInfo.GetPlayerTeamInfo().Equals(teamInfo))
                                {
                                    print($"{teamInfo.DisplayName} existed the (client) lobby");
                                    Destroy(playerInfo.gameObject);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private bool HandleOnPlayerDisconnected(int teamId, string displayName)
    {
        
        if (teams[teamId])
        {
            Transform team = teams[teamId];
            PlayerInfo[] playerInfos = team.GetComponentsInChildren<PlayerInfo>();
            if (lobbyPanel.gameObject.activeInHierarchy)
            {
                if (playerInfos != null && playerInfos.Length > 0)
                {
                    foreach (PlayerInfo playerInfo in playerInfos)
                    {
                        if (playerInfo != null)
                        {
                            if (playerInfo.GetPlayerTeamInfo().DisplayName==displayName)
                            {
                                Destroy(playerInfo.gameObject);
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
    /*private void DisplayPlayerToLobby(PlayerTeamInfo teamInfo) {
        if (teamInfo.teamId >= 0 && lobbyPanel.gameObject.activeInHierarchy) {
            GameObject playerInfoObj = Instantiate(playerInfoPrefabs[teamInfo.teamId]);
            playerInfoObj.transform.parent = teams[teamInfo.teamId];

            PlayerInfo playerInfo = playerInfoObj.GetComponent<PlayerInfo>();
            playerInfo.SetPlayerTeamInfo(teamInfo);
        }
    }*/

    private void DisplayPlayerToLobby(PlayerTeamInfo[] teamInfo) {
        List<PlayerTeamInfo> team0 = new List<PlayerTeamInfo>();
        List<PlayerTeamInfo> team1 = new List<PlayerTeamInfo>();


        for (int i = 0; i < teamInfo.Length; i++) {
            if (teamInfo[i].teamId == 0) {
                team0.Add(teamInfo[i]);
            }

            if (teamInfo[i].teamId == 1)
            {
                team1.Add(teamInfo[i]);
            }
        }

        for (int i = 0; i < team1Info.Length; i++) {
            bool isActive = i < team0.Count;
            team1Info[i].gameObject.SetActive(isActive);

            if (isActive) {
                team1Info[i].SetPlayerTeamInfo(team0[i]);
            }
        }

        for (int i = 0; i < team2Info.Length; i++) {
            bool isActive = i < team1.Count;
            team2Info[i].gameObject.SetActive(isActive);
            if (isActive)
            {
                team2Info[i].SetPlayerTeamInfo(team1[i]);
            }
        }

    }

    private void ClearClientLobby() {
        for (int j = 0; j < team1Info.Length; j++) {
            team1Info[j].gameObject.SetActive(false);
        }
        for (int j = 0; j < team2Info.Length; j++)
        {
            team2Info[j].gameObject.SetActive(false);
        }
    }
}
