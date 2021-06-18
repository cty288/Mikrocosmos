using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ProfilesModels;
using Polyglot;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Launcher : RootPanel {
    //TODO: auto login (session ticket save)

    public static Launcher _instance;

    [SerializeField]
    private Button[] panelButtons;

    [SerializeField]
    private GameObject[] panels;

    [SerializeField] 
    private GameObject noInternetPanel;



    void Awake() {
        _instance = this;

        Screen.SetResolution(1280,720,false);
        SelectPanel(SearchButtonIndex("Login Button"));
        

    }

    void Start() {
        panelButtons[SearchButtonIndex("Register Button")].onClick.AddListener(OnRegisterButtonClicked);
        panelButtons[SearchButtonIndex("Settings Button")].onClick.AddListener(OnSettingsButtonClicked);
        panelButtons[SearchButtonIndex("Login Button")].onClick.AddListener(OnLoginButtonClicked);

        EventCenter.AddListener(EventType.LAUNCHER_OnLoginPanelLoginSuccess,OpenGame);
        EventCenter.AddListener<string>(EventType.LAUNCHER_Error_Message, SetErrorMessage);
    }

    void OnDestroy() {
        EventCenter.RemoveListener(EventType.LAUNCHER_OnLoginPanelLoginSuccess, OpenGame);
        EventCenter.RemoveListener<string>(EventType.LAUNCHER_Error_Message, SetErrorMessage);
    }

    private void OnRegisterButtonClicked() {
        SelectPanel(SearchButtonIndex("Register Button"));
    }

    private void OnSettingsButtonClicked() {
        SelectPanel(SearchButtonIndex("Settings Button"));
    }

    private void OnLoginButtonClicked()
    {
        SelectPanel(SearchButtonIndex("Login Button"));
    }

    private int SearchButtonIndex(string n) {
        for (int i = 0; i < panelButtons.Length; i++)
        {
            if (panelButtons[i].gameObject.name == n) {
                return i;
            }
        }

        return -1;
    }

    private GameObject SearchPanel(string n) {
        foreach (GameObject panel in panels) {
            if (panel.name == n) {
                return panel;
            }
        }

        return null;
    }

    private void SelectPanel(int index) {
        for (int i = 0; i < panelButtons.Length; i++) {
            if (index == i) {
                panelButtons[i].interactable = false;
                panels[i].SetActive(true);
            }
            else {
                panelButtons[i].interactable = true;
                panels[i].SetActive(false);
            }
        }
    }



    protected override void HandleOnInternetRecovered() {
        noInternetPanel.SetActive(false);
    }

    protected override void HandleOnInternetLost() {
        noInternetPanel.SetActive(true);
    }

    /// <summary>
    /// Save Session Ticket, entity id and playfab id.
    /// </summary>
    /// <param name="sessionTicket"></param>
    /// <param name="entityId"></param>
    public void SaveLoginToken(string sessionTicket, string entityId, Action onSaveSuccess,Action onSaveFailed) {
        PlayerPrefs.SetString("Session_Ticket", sessionTicket);
        PlayerPrefs.SetString("Entity_Id", entityId);
        PlayfabUtilities.SetPlayfabIdFromSessionTicket(sessionTicket, onSaveSuccess, onSaveFailed);
    }



    private bool loginCancelled = false;
    /// <summary>
    /// Login by username and password
    /// </summary>
    /// <param name="username">username</param>
    /// <param name="pwd">password</param>
    public void Login(string username,string pwd) {
        loginCancelled = false;
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest()
        {
            Username = username,
            Password = pwd
        }, result => {
            if (!loginCancelled) {
                SaveLoginToken(result.SessionTicket, result.EntityToken.Entity.Id,
                    () => {
                        EventCenter.Broadcast(EventType.LAUNCHER_OnPlayFabLoginSuccess);
                        infoPanel.DisableCloseButton();
                    }, () => {
                        EventCenter.Broadcast(EventType.LAUNCHER_OnPlayFabLoginFailed,
                             new PlayFabError { Error = PlayFabErrorCode.ConnectionError});
                    });
            }
        }, error => {
            //onPlayfabLoginFailed?.Invoke(error);
            EventCenter.Broadcast(EventType.LAUNCHER_OnPlayFabLoginFailed);
        });
    }


    /// <summary>
    /// Open the main game
    /// </summary>
    public void OpenGame()
    {
        PlayfabTokenPasser._instance.SaveToken(PlayfabUtilities.GetSessionTicket(),PlayfabUtilities.GetEntityId(),
            PlayfabUtilities.GetPlayFabIdFromPlayerPrefs());
        CloseInfoPanel();
        SceneManager.LoadSceneAsync("Menu");
    }

    /// <summary>
    /// Cancel the login process (if the player is currently loging)
    /// </summary>
    public void CancelLogin() {
        loginCancelled = true;
    }


    /// <summary>
    /// Verify if the email account exists in the game's system
    /// </summary>
    /// <param name="email"></param>
    /// <param name="onEmailFind">event invoked when successfully find the corresponding account</param>
    /// <param name="onEmailNotFound">event invoked when failed to find the account</param>
    public void VerifyEmailAccountExistence(string email, Action<string> onEmailFind, Action onEmailNotFound) {
        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest {
            Email = email,
            Password = "aewafo3j1o2jpjopfe"
        }, result => {
            onEmailFind?.Invoke(email);
        }, error => {
            print(error.Error.ToString());
            if (error.Error == PlayFabErrorCode.InvalidEmailOrPassword) {
                onEmailFind?.Invoke(email);
            }
            else {
                onEmailNotFound?.Invoke();
            }
        });
    }

    
}
