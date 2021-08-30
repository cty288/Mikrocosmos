using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Event;
using Mirror;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using EventType = MikroFramework.Event.EventType;


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
        AddListener(EventType.MENU_OnNewUserEnterMenu, HandleOpenNewUserPanel);
        AddListener(EventType.MENU_OnUserEnterMenu,HandleOnUserEnterMenu);
        AddListener(EventType.MENU_AuthorityOnConnected,HandleOnEnterMasterServerSuccess);
        
        
        AddListener(EventType.MENU_Error, SetErrorMessage);


        AddListener(EventType.MENU_MATCHMAKING_ClientRequestingMatchmaking,
            HandleClientRequestMatchmaking);
        AddListener(EventType.MENU_MATCHMAKING_ClientMatchmakingFailed,HandleMatchmakingFailed);

        AddListener(EventType.MENU_MATCHMAKING_ClientMatchmakingSuccess,HandleClientRequestMatchmakingSuccess);
        AddListener(EventType.MENU_MATCHMAKING_ClientMatchmakingReadyToGet,HandleClientReadyToGetMatch);


        AddListener(EventType.MENU_OnClientLeaveLobbyFailed,HandleLeaveLobbyFailed);
        AddListener(EventType.MENU_OnClientLeaveLobbySuccess,HandleLeaveLobbySuccess);

        AddListener(EventType.MENU_OnClientLobbyStateUpdated, HandleLobbyStateUpdate);
        AddListener(EventType.MENU_OnAuthenticaeteFailed,HandleOnAuthenticateFailed);
        AddListener(EventType.MENU_OnClientReceiveServerStartingProcessFailed, HanleOnClientReceiveServerStartingProcessFailed);
    }

  
    private void HandleOnAuthenticateFailed(MikroMessage msg)
    {
        SetErrorMessage("MENU_AUTHENTICATE_FAILED",()=> {
            Application.Quit();}, "MENU_LABEL_EXIT");
    }

    private void HanleOnClientReceiveServerStartingProcessFailed(MikroMessage msg)
    {
        StopWaiting();
        gamemodePanel.SetActive(true);
        SetErrorMessage("MENU_SERVER_START_MATCH_FAILED");
    }
    private void HandleLobbyStateUpdate(MikroMessage msg) {
        MatchState matchState = (MatchState) msg.GetMessage(0);
        string ip = msg.GetMessage(1).ToString();
        ushort port =(ushort) msg.GetMessage(2);
        Mode mode=(Mode)msg.GetMessage(3);

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
                Broadcast(EventType.MENU_OnNewUserEnterMenu,null);

            }else {
                PlayfabTokenPasser._instance.SavePlayerName(result.PlayerProfile.DisplayName);
                Broadcast(EventType.MENU_OnUserEnterMenu,null);
            }
        }, error => {
            StopWaiting();
            Broadcast(EventType.MENU_Error, MikroMessage.Create("ERROR_NETWORK_SERVER", (UnityAction) CheckFirstTimeUser, "MENU_RETRY",null));
            print("[MenuManager] "+ error.Error.ToString());
        });
    }


    #region EventHandlers
    private void HandleStartLoadingCircle(bool hasloadingMessage = false, bool hasLoadingPeriod = false, string
        loadingMessage = "") {
        loadingCircle.StartLoadingCircle(hasloadingMessage,hasLoadingPeriod,loadingMessage);
    }

    private void HandleStopLoadingCircle()
    {
        loadingCircle.StopLoadingCircle();
    }


    private void HandleOpenNewUserPanel(MikroMessage msg)
    {
        if (firstTimeUserPanel.gameObject) {
            firstTimeUserPanel.gameObject.SetActive(true);
        }
        
    }

    private void HandleOnUserEnterMenu(MikroMessage msg) {
        print("Old User enter game");
        OpenInfoPanel("INTERNET_CONNECTING_TO_SERVER", true);
        //MIRROR_OnMirrorConnectSuccess and MIRROR_OnMirrorConnectTimeout will be triggered
        //connect to server using NetworkConnector
        NetworkConnector._singleton.ConnectToServer(ServerInfo.ServerIp,ServerInfo.MasterServerPort,null, HandleOnEnterMasterServerFailed);
    }

    private void HandleOnEnterMasterServerSuccess(MikroMessage msg) {
        CloseInfoPanel();
        if (gamemodePanel) {
            gamemodePanel.SetActive(true);
        }

        print("Connect to master server");
    }

    private void HandleOnEnterMasterServerFailed(MikroMessage msg) {
        CloseInfoPanel();
        Broadcast(EventType.MENU_Error, MikroMessage.Create("ERROR_NETWORK_SERVER", (Action<MikroMessage>)HandleOnUserEnterMenu, "MENU_RETRY", null));
    }

    private void HandleMatchmakingFailed(MikroMessage msg) {
        StopWaiting();
        Broadcast(EventType.MENU_Error, MikroMessage.Create("MENU_ERROR_MATCHMAKING_FAILED", (Action) (() => { }), "GAME_ACTION_CLOSE", null));
        
        if (cancelMatchmakingButton) {
            cancelMatchmakingButton.gameObject.SetActive(false);
        }

    }

    private void HandleClientRequestMatchmaking(MikroMessage msg) {

        bool hasloadingMessage = (bool) msg.GetMessage(0);
        bool hasLoadingPeriod = (bool) msg.GetMessage(1);
        string loadingMessage = msg.GetMessage(2).ToString();


        HandleStartLoadingCircle(hasloadingMessage,hasLoadingPeriod,loadingMessage);

        if (cancelMatchmakingButton) {
            cancelMatchmakingButton.gameObject.SetActive(true);
            cancelMatchmakingButton.interactable = true;
        }


    }

    private void HandleClientReadyToGetMatch(MikroMessage msg) {
        if (cancelMatchmakingButton)
        {
            loadingCircle.ChangeLoadingMessage(true, "MENU_WAITING_FIND_MATCH");
            cancelMatchmakingButton.gameObject.SetActive(true);
            cancelMatchmakingButton.interactable = false;
        }
    }

    private void HandleClientRequestMatchmakingSuccess(MikroMessage msg) {
        PlayerTeamInfo thisPlayerTeamInfo = msg.GetSingleMessage() as PlayerTeamInfo;
        StopWaiting();
        
        if (cancelMatchmakingButton) {
            cancelMatchmakingButton.gameObject.SetActive(false);
            cancelMatchmakingButton.interactable = false;
        }
        Debug.Log($"[MenuManager] Client request new match room success! Matchid: {thisPlayerTeamInfo.matchId}");
        //lobby panel setup
        if (gamemodePanel) {
            gamemodePanel.SetActive(false);
        }
       
        
    }

    private void HandleLeaveLobbySuccess(MikroMessage msg) {
        StopWaiting();
        gamemodePanel.SetActive(true);
    }

    private void HandleLeaveLobbyFailed(MikroMessage msg) {
        MatchError error =(MatchError) msg.GetSingleMessage();

        StopWaiting();
        if (error == MatchError.MatchAlreadyStart) {
            SetErrorMessage("MENU_ERROR_MATCH_ALREADY_START");
        }else if (error == MatchError.UnableToFindMatch || error == MatchError.UnableToFindPlayer) {
            gamemodePanel.SetActive(true);
            SetErrorMessage("MENU_ERROR_MATCH_EXIT_ERROR");
        }
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
        Broadcast(EventType.MENU_MATCHMAKING_ClientMatchmakingCancelled,null);
        if (cancelMatchmakingButton) {
            cancelMatchmakingButton.gameObject.SetActive(false);
            StopWaiting();
        }
    }
}
