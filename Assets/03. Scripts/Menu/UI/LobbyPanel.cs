using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Event;
using Mirror;
using Polyglot;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EventType = MikroFramework.Event.EventType;

public class LobbyPanel : MikroBehavior {
    [SerializeField] private LobbyPanelChild lobbyPanel;

    [SerializeField] private PlayerInfo[] team1Info;
    [SerializeField] private PlayerInfo[] team2Info;

    [SerializeField] private GameObject gamestartInfo;
    [SerializeField] private Button leaveLobbyButton;
    [Tooltip("Each prefab corresponds to each team in the teams array")]
    [SerializeField] private GameObject[] playerInfoPrefabs;

    private float countDown = 10f;
    private bool countDownStart = false;

    void Awake() {
        
        AddListener(EventType.MENU_MATCHMAKING_ClientMatchmakingSuccess,
            HandleClientJoinLobby);
        AddListener(EventType.MENU_OnClientLeaveLobbySuccess, HandleLeaveLobbySuccess);
        AddListener(EventType.MENU_OnClientLeaveLobbyFailed,HandleLeaveLobbyFailed);

        LobbyPanelChild.onLobbyPanelChildEnabled += AddLobbyPanelChildListeners;
        LobbyPanelChild.onLobbyPanelChildDisabled += RemoveLobbyPanelChildListeners;
        // EventCenter.AddListener<PlayerTeamInfo>(EventType.MENU_OnClientNewPlayerJoinLobby,HandleOnNewPlayerJoinLobby);
        // EventCenter.AddListener<PlayerTeamInfo>(EventType.MENU_OnClientPlayerDisconnected,HandleOnPlayerDisconnected);
        leaveLobbyButton.onClick.AddListener(OnLeaveLobbyButtonClicked);
    }

   

    protected override void OnBeforeDestroy() {
        
    }

    private void AddLobbyPanelChildListeners() {
        AddListener(EventType.MENU_OnClientLobbyInfoUpdated,DisplayPlayerToLobby);
        AddListener(EventType.MENU_OnClientLobbyStateUpdated,HandleLobbyStateUpdate);
        AddListener(EventType.MENU_OnClientLobbyCountdownUpdated,HandleOnCountdownUpdate);
    }

    private void RemoveLobbyPanelChildListeners() {
        RemoveListener(EventType.MENU_OnClientLobbyInfoUpdated, DisplayPlayerToLobby);
        RemoveListener(EventType.MENU_OnClientLobbyStateUpdated, HandleLobbyStateUpdate);
        RemoveListener(EventType.MENU_OnClientLobbyCountdownUpdated, HandleOnCountdownUpdate);
    }

    private void HandleClientJoinLobby(MikroMessage msg) {
        PlayerTeamInfo thisPlayerInfo = msg.GetSingleMessage() as PlayerTeamInfo;
        

        if (lobbyPanel) {
            lobbyPanel.gameObject.SetActive(true);
            ClearClientLobby();
        }
    }


    private void DisplayPlayerToLobby(MikroMessage msg) {

        PlayerTeamInfo[] teamInfo = msg.GetMessage(0) as PlayerTeamInfo[];
        PlayerTeamInfo myInfo = msg.GetMessage(1) as PlayerTeamInfo;

        List<PlayerTeamInfo> team0 = new List<PlayerTeamInfo>();
        List<PlayerTeamInfo> team1 = new List<PlayerTeamInfo>();


        for (int i = 0; i < teamInfo.Length; i++) {
            if (teamInfo[i].username == myInfo.username) {
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

    private void HandleLobbyStateUpdate(MikroMessage msg) {
        MatchState matchState = (MatchState)msg.GetMessage(0);
        string ip=msg.GetMessage(1).ToString();
        ushort port=(ushort)msg.GetMessage(2);
        Mode gamemode=(Mode)msg.GetMessage(3);


        switch (matchState) {
            case MatchState.WaitingForPlayers:
                SetLeaveLobbyButton(true,true);
                StopCountDown();
                break;
            case MatchState.CountDownForMatch:
                SetLeaveLobbyButton(true,true);
                StartCountDown();
                break;
            case MatchState.StartingGameProcess:
                SetLeaveLobbyButton(false,false);
                print("Client starting game process...");
                ShowReadyInfo();
                break;
            case MatchState.GameAlreadyStart:
                SetLeaveLobbyButton(false,false);
                StopCountDown();
                PlayerPrefs.SetString("ip",ip);
                PlayerPrefs.SetInt("port",port);
                SceneManager.LoadSceneAsync(ServerInfo.GameModeSceneName[(int) gamemode]);
                break;
            case MatchState.MatchSpawnFailed:
                OnJoinGameServerFailed();
                break;
        }
    }

    private void OnJoinGameServerFailed() {
        Broadcast(EventType.MENU_OnClientReceiveServerStartingProcessFailed,null);
        if (lobbyPanel)
        {
            lobbyPanel.gameObject.SetActive(false);
        }
    }

    private void StartCountDown() {
        countDownStart = true;
        
    }

    private void StopCountDown() {
        countDownStart = false;
        gamestartInfo.SetActive(false);
    }

    private void ShowReadyInfo() {
        countDownStart = false;
        gamestartInfo.GetComponentInChildren<Text>().text =
            Localization.Get("MENU_READY_INFO");
    }

    void Update() {
        if (countDownStart) {
            gamestartInfo.SetActive(true);

            countDown -= Time.deltaTime;
            
            if (countDown <= 0) {
                countDown = 0;
            }

            int countDownToInt = Mathf.RoundToInt(countDown);

            gamestartInfo.GetComponentInChildren<Text>().text =
                Localization.GetFormat("LOBBY_START_INFO", countDownToInt.ToString());

        }
    }

   

    private void SetLeaveLobbyButton(bool isActive, bool isInteractable) {
        if (leaveLobbyButton) {
            leaveLobbyButton.interactable = isInteractable;
            leaveLobbyButton.gameObject.SetActive(isActive);
        }
    }

    private void OnLeaveLobbyButtonClicked() {
        MasterServerPlayer player = NetworkClient.connection.identity.GetComponent<MasterServerPlayer>();
        if (player) {
            player.ClientRequestLeaveLobby();
            MenuManager._instance.StartWaiting(true,true, "MENU_WAITING_LEAVE_LOBBY");
        }
    }

    private void HandleLeaveLobbySuccess(MikroMessage msg) {
        lobbyPanel.gameObject.SetActive(false);
    }

    private void HandleLeaveLobbyFailed(MikroMessage msg) {
        MatchError error =(MatchError) msg.GetSingleMessage();

        if (error == MatchError.MatchAlreadyStart) {
            return;
        }
        else if (error == MatchError.UnableToFindMatch || error == MatchError.UnableToFindPlayer)
        {
            lobbyPanel.gameObject.SetActive(false);
        }
    }

    private void HandleOnCountdownUpdate(MikroMessage msg) {
        float countDown =(float) msg.GetSingleMessage();
        /*if (gamestartInfo.activeInHierarchy) {
            gamestartInfo.GetComponentInChildren<Text>().text =
                Localization.GetFormat("LOBBY_START_INFO", countDown.ToString());
        }*/
        this.countDown = countDown;
    }
}
