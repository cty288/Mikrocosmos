using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.SceneManagement;
using EventType = MikroFramework.Event.EventType;

public class LoadGamePanel : RootPanel
{
    [SerializeField] private LoadingCircle loadingCircle;

    void Awake() {
        AddListener(EventType.GAME_OnClientConnectingToServer,HandleOnConnectingToServer);
        AddListener(EventType.GAME_OnClientConnectingToServerFailed, HandleOnConnectingToServerFailed);
        AddListener(EventType.GAME_OnClientConnectingToServerSuccess, HandleOnConnectingToServerSuccess);
        AddListener(EventType.GAME_OnClientAuthenticated,HandleOnClientAuthenticated);
    }

  

    private void HandleOnClientAuthenticated(MikroMessage msg) {
        bool isSuccess = (bool) msg.GetSingleMessage();
        if (isSuccess) {
            Debug.Log("Load Game Panel: Authenticate success");
        }
        else {
            SetErrorMessage("GAME_AUTHENTICATE_FAILED", BackToMenu, "MENU_RETURN_TO_MENU_BUTTON");
        }
    }

    private void HandleOnConnectingToServer(MikroMessage msg) {
        StartLoadingCircle(true,true, "MENU_ENTER_GAME_LOADING");
    }

    private void HandleOnConnectingToServerFailed(MikroMessage msg) {
        SetErrorMessage("MENU_ENTER_GAME_Failed", BackToMenu, "MENU_RETURN_TO_MENU_BUTTON");
    }

    private void HandleOnConnectingToServerSuccess(MikroMessage msg) {
        StopLoadingCircle();
    }

    private void BackToMenu() {
        HandleOnConnectingToServer(null);
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
