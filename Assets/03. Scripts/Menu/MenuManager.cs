using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Client Class 
/// </summary>
public class MenuManager : RootPanel {
    public static MenuManager _instance;

    [SerializeField] private GameObject firstTimeUserPanel;
    [SerializeField] private LoadingCircle loadingCircle;

    void Awake() {
        _instance = this;
        Screen.SetResolution(1920, 1080, true);
        Screen.fullScreen = true;

        OnStartAddListeners();
    }

    void OnDestroy()
    {
        OnDestroyRemoveListeners();
    }

    void Start()
    {
        firstTimeUserPanel.gameObject.SetActive(false);
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
        EventCenter.AddListener(EventType.MIRROR_OnMirrorConnectSuccess,HandleOnEnterMasterServerSuccess);
        EventCenter.AddListener(EventType.MIRROR_OnMirrorConnectTimeout, HandleOnEnterMasterServerFailed);

        EventCenter.AddListener<string, UnityAction, string, object[]>(EventType.MENU_Error, SetErrorMessage);
        EventCenter.AddListener(EventType.MENU_WaitingNetworkResponse,HandleStartLoadingCircle);
        EventCenter.AddListener(EventType.MENU_StopWaitingNetworkResponse,HandleStopLoadingCircle);
    }

    private void OnDestroyRemoveListeners()
    {
        EventCenter.RemoveListener(EventType.MENU_OnNewUserEnterMenu, HandleOpenNewUserPanel);
        EventCenter.RemoveListener(EventType.MENU_OnUserEnterMenu, HandleOnUserEnterMenu);

        EventCenter.RemoveListener(EventType.MENU_WaitingNetworkResponse, HandleStartLoadingCircle);
        EventCenter.RemoveListener(EventType.MENU_StopWaitingNetworkResponse, HandleStopLoadingCircle);
        EventCenter.RemoveListener<string, UnityAction, string, object[]>(EventType.MENU_Error, SetErrorMessage);

        EventCenter.RemoveListener(EventType.MIRROR_OnMirrorConnectSuccess, HandleOnEnterMasterServerSuccess);
        EventCenter.RemoveListener(EventType.MIRROR_OnMirrorConnectTimeout, HandleOnEnterMasterServerFailed);
    }

    void Update()
    {
        
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
                EventCenter.Broadcast(EventType.MENU_OnUserEnterMenu);
            }
        }, error => {
            StopWaiting();
            EventCenter.Broadcast<string, UnityAction, string, object[]>(EventType.MENU_Error, "ERROR_NETWORK_SERVER", CheckFirstTimeUser, "MENU_RETRY",null);
            print(error.Error.ToString());
        });
    }


    #region EventHandlers
    private void HandleStartLoadingCircle()
    {
        loadingCircle.StartLoadingCircle();
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
        NetworkConnector._singleton.ConnectToServer(ServerInfo.ServerIp,ServerInfo.MasterServerPort);
    }

    private void HandleOnEnterMasterServerSuccess() {
        CloseInfoPanel();
        print("Connect to master server");
    }

    private void HandleOnEnterMasterServerFailed() {
        CloseInfoPanel();
        EventCenter.Broadcast<string, UnityAction, string, object[]>(EventType.MENU_Error, "ERROR_NETWORK_SERVER", HandleOnUserEnterMenu, "MENU_RETRY", null);
    }
    #endregion



    /// <summary>
    /// Start Waiting (open loading circle)
    /// </summary>
    public void StartWaiting()
    {
        EventCenter.Broadcast(EventType.MENU_WaitingNetworkResponse);
    }

    /// <summary>
    /// Stop Waiting Circle
    /// </summary>
    public void StopWaiting()
    {
        EventCenter.Broadcast(EventType.MENU_StopWaitingNetworkResponse);
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
}
