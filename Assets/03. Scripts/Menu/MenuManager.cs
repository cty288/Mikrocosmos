using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Client Class 
/// </summary>
public class MenuManager : RootPanel {
    public static MenuManager _instance;

    [SerializeField] private GameObject firstTimeUserPanel;
    [SerializeField] private LoadingCircle loadingCircle;
    [SerializeField] private GameObject gamemodePanel;
    [SerializeField] private Button cancelMatchmakingButton;
    void Awake() {
        _instance = this;
        Screen.SetResolution(1280, 720, false);
        //Screen.fullScreen = true;

        OnStartAddListeners();
    }

    void OnDestroy()
    {
        OnDestroyRemoveListeners();
    }

    void Start()
    {
        firstTimeUserPanel.gameObject.SetActive(false);
        cancelMatchmakingButton.onClick.AddListener(OnCancelMatchmakingButtonClicked);
        StartCoroutine(LateStart());
    }
    /// <summary>
    /// For some weird reason, these methods need to be called a slightly later after the scene loaded
    /// </summary>
    /// <returns></returns>
    private IEnumerator LateStart() {
        yield return new WaitForSeconds(0.3f);
        CheckFirstTimeUser();
    }

    private void OnStartAddListeners() {
        EventCenter.AddListener(EventType.MENU_OnNewUserEnterMenu, HandleOpenNewUserPanel);
        EventCenter.AddListener(EventType.MENU_OnUserEnterMenu,HandleOnUserEnterMenu);
        EventCenter.AddListener(EventType.MENU_AuthorityOnConnected,HandleOnEnterMasterServerSuccess);
        
        
        EventCenter.AddListener<string, UnityAction, string, object[]>(EventType.MENU_Error, SetErrorMessage);


        EventCenter.AddListener<bool,bool,string>(EventType.MENU_MATCHMAKING_ClientRequestingMatchmaking,
            HandleClientRequestMatchmaking);
        EventCenter.AddListener(EventType.MENU_MATCHMAKING_ClientMatchmakingFailed,HandleMatchmakingFailed);
        
        EventCenter.AddListener<PlayerTeamInfo>(EventType.MENU_MATCHMAKING_ClientMatchmakingSuccess,HandleClientRequestMatchmakingSuccess);
        EventCenter.AddListener(EventType.MENU_MATCHMAKING_ClientMatchmakingReadyToGet,HandleClientReadyToGetMatch);


        EventCenter.AddListener<MatchError>(EventType.MENU_OnClientLeaveLobbyFailed,HandleLeaveLobbyFailed);
        EventCenter.AddListener(EventType.MENU_OnClientLeaveLobbySuccess,HandleLeaveLobbySuccess);

        EventCenter.AddListener<MatchState, string, ushort, Mode>(EventType.MENU_OnClientLobbyStateUpdated, HandleLobbyStateUpdate);
        EventCenter.AddListener(EventType.MENU_OnClientReceiveServerStartingProcessFailed, HanleOnClientReceiveServerStartingProcessFailed);
    }

    private void OnDestroyRemoveListeners()
    {
        EventCenter.RemoveListener(EventType.MENU_OnNewUserEnterMenu, HandleOpenNewUserPanel);
        EventCenter.RemoveListener(EventType.MENU_OnUserEnterMenu, HandleOnUserEnterMenu);

        EventCenter.RemoveListener<string, UnityAction, string, object[]>(EventType.MENU_Error, SetErrorMessage);
        EventCenter.RemoveListener<bool, bool, string>(EventType.MENU_MATCHMAKING_ClientRequestingMatchmaking,
            HandleClientRequestMatchmaking);
        EventCenter.RemoveListener(EventType.MENU_AuthorityOnConnected, HandleOnEnterMasterServerSuccess);
        
        EventCenter.RemoveListener(EventType.MENU_MATCHMAKING_ClientMatchmakingFailed, HandleMatchmakingFailed);
        EventCenter.RemoveListener<PlayerTeamInfo>(EventType.MENU_MATCHMAKING_ClientMatchmakingSuccess, HandleClientRequestMatchmakingSuccess);
        EventCenter.RemoveListener(EventType.MENU_MATCHMAKING_ClientMatchmakingReadyToGet, HandleClientReadyToGetMatch);
        EventCenter.RemoveListener<MatchError>(EventType.MENU_OnClientLeaveLobbyFailed, HandleLeaveLobbyFailed);
        EventCenter.RemoveListener(EventType.MENU_OnClientLeaveLobbySuccess, HandleLeaveLobbySuccess);
        EventCenter.RemoveListener<MatchState, string, ushort, Mode>(EventType.MENU_OnClientLobbyStateUpdated, HandleLobbyStateUpdate);
        EventCenter.RemoveListener(EventType.MENU_OnClientReceiveServerStartingProcessFailed, HanleOnClientReceiveServerStartingProcessFailed);
    }

    private void HandleLobbyStateUpdate(MatchState matchState, string ip, ushort port, Mode mode) {
        if (matchState == MatchState.GameAlreadyStart) {
            StartWaiting(true,true, "MENU_ENTER_GAME_LOADING");
        }
    }

    private void CheckFirstTimeUser(){
        StartWaiting();
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest
        {
            PlayFabId = PlayfabTokenPasser._instance.Token.PlayfabId
        }, result => {
            StopWaiting();
            if (result.PlayerProfile.DisplayName == null) {
                EventCenter.Broadcast(EventType.MENU_OnNewUserEnterMenu);

            }else {
                PlayfabTokenPasser._instance.SavePlayerName(result.PlayerProfile.DisplayName);
                EventCenter.Broadcast(EventType.MENU_OnUserEnterMenu);
            }
        }, error => {
            StopWaiting();
            EventCenter.Broadcast<string, UnityAction, string, object[]>(EventType.MENU_Error, "ERROR_NETWORK_SERVER", CheckFirstTimeUser, "MENU_RETRY",null);
            print(error.Error.ToString());
        });
    }


    #region EventHandlers
    private void HandleStartLoadingCircle(bool hasloadingMessage = false, bool hasLoadingPeriod = false, string
        loadingMessage = "")
    {
        loadingCircle.StartLoadingCircle(hasloadingMessage,hasLoadingPeriod,loadingMessage);
    }

    private void HandleStopLoadingCircle()
    {
        loadingCircle.StopLoadingCircle();
    }


    private void HandleOpenNewUserPanel()
    {
        if (firstTimeUserPanel.gameObject) {
            firstTimeUserPanel.gameObject.SetActive(true);
        }
        
    }

    private void HandleOnUserEnterMenu() {
        print("Old User enter game");
        OpenInfoPanel("INTERNET_CONNECTING_TO_SERVER", true);
        //MIRROR_OnMirrorConnectSuccess and MIRROR_OnMirrorConnectTimeout will be triggered
        //connect to server using NetworkConnector
        NetworkConnector._singleton.ConnectToServer(ServerInfo.ServerIp,ServerInfo.MasterServerPort,null,HandleOnEnterMasterServerFailed);
    }

    private void HandleOnEnterMasterServerSuccess() {
        CloseInfoPanel();
        if (gamemodePanel) {
            gamemodePanel.SetActive(true);
        }

        print("Connect to master server");
    }

    private void HandleOnEnterMasterServerFailed() {
        CloseInfoPanel();
        EventCenter.Broadcast<string, UnityAction, string, object[]>(EventType.MENU_Error, "ERROR_NETWORK_SERVER", HandleOnUserEnterMenu, "MENU_RETRY", null);
    }

    private void HandleMatchmakingFailed() {
        StopWaiting();
        EventCenter.Broadcast<string, UnityAction, string, object[]>
            (EventType.MENU_Error, "MENU_ERROR_MATCHMAKING_FAILED", () => { }, "GAME_ACTION_CLOSE", null);
        
        if (cancelMatchmakingButton) {
            cancelMatchmakingButton.gameObject.SetActive(false);
        }

    }

    private void HandleClientRequestMatchmaking(bool hasloadingMessage = false, bool hasLoadingPeriod = false, string
        loadingMessage = "") {
        HandleStartLoadingCircle(hasloadingMessage,hasLoadingPeriod,loadingMessage);

        if (cancelMatchmakingButton) {
            cancelMatchmakingButton.gameObject.SetActive(true);
            cancelMatchmakingButton.interactable = true;
        }


    }

    private void HandleClientReadyToGetMatch() {
        if (cancelMatchmakingButton)
        {
            loadingCircle.ChangeLoadingMessage(true, "MENU_WAITING_FIND_MATCH");
            cancelMatchmakingButton.gameObject.SetActive(true);
            cancelMatchmakingButton.interactable = false;
        }
    }

    private void HandleClientRequestMatchmakingSuccess(PlayerTeamInfo thisPlayerTeamInfo) {
        StopWaiting();
        
        if (cancelMatchmakingButton) {
            cancelMatchmakingButton.gameObject.SetActive(false);
            cancelMatchmakingButton.interactable = false;
        }
        Debug.Log($"Client request new match room success! Matchid: {thisPlayerTeamInfo.matchId}");
        //lobby panel setup
        if (gamemodePanel) {
            gamemodePanel.SetActive(false);
        }
       
        
    }

    private void HandleLeaveLobbySuccess() {
        StopWaiting();
        gamemodePanel.SetActive(true);
    }

    private void HandleLeaveLobbyFailed(MatchError error) {
        StopWaiting();
        if (error == MatchError.MatchAlreadyStart) {
            SetErrorMessage("MENU_ERROR_MATCH_ALREADY_START");
        }else if (error == MatchError.UnableToFindMatch || error == MatchError.UnableToFindPlayer) {
            gamemodePanel.SetActive(true);
            SetErrorMessage("MENU_ERROR_MATCH_EXIT_ERROR");
        }
    }

    private void HanleOnClientReceiveServerStartingProcessFailed(){
        StopWaiting();
        gamemodePanel.SetActive(true);
        SetErrorMessage("MENU_SERVER_START_MATCH_FAILED");
    }
    #endregion



    /// <summary>
    /// Start Waiting (open loading circle)
    /// </summary>
    public void StartWaiting(bool hasloadingMessage = false, bool hasLoadingPeriod = false, string
        loadingMessageLocalized = "")
    {
        HandleStartLoadingCircle(hasloadingMessage,hasLoadingPeriod,loadingMessageLocalized);
    }

    /// <summary>
    /// Stop Waiting Circle
    /// </summary>
    public void StopWaiting()
    {
        HandleStopLoadingCircle();
    }

    public void UpdateAndSaveDisplayName(string newName,Action<string> onSuccess,Action<PlayFabError> onFailed) {
        PlayfabUtilities.UpdateDisplayName(newName,
            (displayName) => {
                PlayfabTokenPasser._instance.SavePlayerName(displayName);
                onSuccess?.Invoke(displayName);
            }, error => {
                MenuManager._instance.StopWaiting();
                onFailed?.Invoke(error);
            });
    }

    /// <summary>
    /// Handle a weird Unity bug when calling coroutine on a singleton
    /// When call a coroutine using MenuManager, always use this method
    /// Example: StartCoroutine(MenuManager.GetOrCreate(gameObject).xxx)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static MenuManager GetOrCreate(GameObject gameObject) {
        if (!gameObject) {
            return new MenuManager();
        }

        var existed = gameObject.GetComponent<MenuManager>();
        return existed ?? gameObject.AddComponent<MenuManager>();
    }

    /// <summary>
    /// Request the server to find a match. The server will try to first find an existing and available one
    /// If the server can't find it, it will create a new lobby room (via Playfab Matchmaking)
    /// </summary>
    /// <param name="gamemode"></param>

    public void RequestMatch(Mode gamemode) {
        NetworkClient.connection.identity.GetComponent<MasterServerPlayer>().RequestMatch(gamemode);
    }

    private void OnCancelMatchmakingButtonClicked() {
        EventCenter.Broadcast(EventType.MENU_MATCHMAKING_ClientMatchmakingCancelled);
        if (cancelMatchmakingButton) {
            cancelMatchmakingButton.gameObject.SetActive(false);
            StopWaiting();
        }
    }


}
