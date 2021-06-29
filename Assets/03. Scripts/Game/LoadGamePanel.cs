using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGamePanel : RootPanel
{
    [SerializeField] private LoadingCircle loadingCircle;

    void Awake() {
        EventCenter.AddListener(EventType.GAME_OnClientConnectingToServer,HandleOnConnectingToServer);
        EventCenter.AddListener(EventType.GAME_OnClientConnectingToServerFailed, HandleOnConnectingToServerFailed);
        EventCenter.AddListener(EventType.GAME_OnClientConnectingToServerSuccess, HandleOnConnectingToServerSuccess);
        EventCenter.AddListener<bool>(EventType.GAME_OnClientAuthenticated,HandleOnClientAuthenticated);
    }

    void OnDestroy() {
        EventCenter.RemoveListener(EventType.GAME_OnClientConnectingToServer, HandleOnConnectingToServer);
        EventCenter.RemoveListener(EventType.GAME_OnClientConnectingToServerFailed, HandleOnConnectingToServerFailed);
        EventCenter.RemoveListener(EventType.GAME_OnClientConnectingToServerSuccess, HandleOnConnectingToServerSuccess);
        EventCenter.RemoveListener<bool>(EventType.GAME_OnClientAuthenticated, HandleOnClientAuthenticated);
    }

    private void HandleOnClientAuthenticated(bool isSuccess) {
        if (isSuccess) {
            Debug.Log("Load Game Panel: Authenticate success");
        }
        else {
            SetErrorMessage("GAME_AUTHENTICATE_FAILED", BackToMenu, "MENU_RETURN_TO_MENU_BUTTON");
        }
    }

    private void HandleOnConnectingToServer() {
        StartLoadingCircle(true,true, "MENU_ENTER_GAME_LOADING");
    }

    private void HandleOnConnectingToServerFailed() {
        SetErrorMessage("MENU_ENTER_GAME_Failed", BackToMenu, "MENU_RETURN_TO_MENU_BUTTON");
    }

    private void HandleOnConnectingToServerSuccess() {
        StopLoadingCircle();
    }

    private void BackToMenu() {
        HandleOnConnectingToServer();
        SceneManager.LoadSceneAsync("Menu");
    }

    private void StartLoadingCircle(bool hasloadingMessage = false, bool hasLoadingPeriod = false, string
        loadingMessage = "")
    {
        loadingCircle.StartLoadingCircle(hasloadingMessage, hasLoadingPeriod, loadingMessage);
    }

    private void StopLoadingCircle()
    {
        loadingCircle.StopLoadingCircle();
    }


}
