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
        EventCenter.AddListener<PlayerTeamInfo[],PlayerTeamInfo>(EventType.MENU_OnClientLobbyInfoUpdated,DisplayPlayerToLobby);
    }

    private void RemoveLobbyPanelChildListeners() {
        EventCenter.RemoveListener<PlayerTeamInfo[], PlayerTeamInfo>(EventType.MENU_OnClientLobbyInfoUpdated, DisplayPlayerToLobby);
    }

    private void HandleClientJoinLobby(PlayerTeamInfo thisPlayerInfo) {
        if (lobbyPanel) {
            lobbyPanel.gameObject.SetActive(true);
            ClearClientLobby();
        }
    }


    private void DisplayPlayerToLobby(PlayerTeamInfo[] teamInfo, PlayerTeamInfo myInfo) {
        List<PlayerTeamInfo> team0 = new List<PlayerTeamInfo>();
        List<PlayerTeamInfo> team1 = new List<PlayerTeamInfo>();


        for (int i = 0; i < teamInfo.Length; i++) {
            if (teamInfo[i].DisplayName == myInfo.DisplayName) {
                continue;
            }

            if (teamInfo[i].teamId == 0) {
                team0.Add(teamInfo[i]);
            }

            if (teamInfo[i].teamId == 1)
            {
                team1.Add(teamInfo[i]);
            }
        }

        if (myInfo.teamId == 0) {
            team0.Insert(0,myInfo);
        }else if (myInfo.teamId == 1)
        {
            team1.Insert(0,myInfo);
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
