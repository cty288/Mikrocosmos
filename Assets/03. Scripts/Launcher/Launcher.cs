using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PlayFab;
using PlayFab.ClientModels;
using Polyglot;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Launcher : MonoBehaviour {
    //TODO: auto login (session ticket save)

    public static Launcher _instance;

    [SerializeField]
    private Button[] panelButtons;

    [SerializeField]
    private GameObject[] panels;

    [SerializeField] 
    private GameObject noInternetPanel;

    [SerializeField] 
    private ErrorPanel errorPanel;

    [SerializeField] private InfoPanel infoPanel;

    /// <summary>
    /// Invoked when login success on playfab
    /// </summary>
    public Action onPlayfabLoginSuccess;
    /// <summary>
    /// Invoked when login failed on Playfab
    /// </summary>
    public Action<PlayFabError> onPlayfabLoginFailed;

    void Awake() {
        _instance = this;

        Screen.SetResolution(1280,720,false);
        SelectPanel(SearchButtonIndex("Login Button"));
        
        panelButtons[SearchButtonIndex("Register Button")].onClick.AddListener(OnRegisterButtonClicked);
        panelButtons[SearchButtonIndex("Settings Button")].onClick.AddListener(OnSettingsButtonClicked);
        panelButtons[SearchButtonIndex("Login Button")].onClick.AddListener(OnLoginButtonClicked);

        InternetConnectivityChecker._instance.onInternetRecovered += HandleOnInternetRecovered;
        InternetConnectivityChecker._instance.onInternetLostConnection += HandleOnInternetLost;
    }

    void OnDestroy() {
        InternetConnectivityChecker._instance.onInternetRecovered -= HandleOnInternetRecovered;
        InternetConnectivityChecker._instance.onInternetLostConnection -= HandleOnInternetLost;
    }
    void Update() {
        
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

    /// <summary>
    /// Set the error message of the error panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="parameters">Parameters of the localized message, if exists</param>
    public void SetErrorMessage(string localizedMessageId, params object[] parameters)
    {
        errorPanel.gameObject.SetActive(true);
        errorPanel.SetErrorMessage(localizedMessageId,parameters);
    }

    /// <summary>
    /// Set the error message of the error panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    public void SetErrorMessage(string localizedMessageId)
    {
        errorPanel.gameObject.SetActive(true);
        errorPanel.SetErrorMessage(localizedMessageId);
    }
    private void HandleOnInternetRecovered() {
        noInternetPanel.SetActive(false);
    }

    private void HandleOnInternetLost() {
        noInternetPanel.SetActive(true);
    }

    /// <summary>
    /// Save Session Ticket, entity id and playfab id. Return true if successfully login
    /// </summary>
    /// <param name="sessionTicket"></param>
    /// <param name="entityId"></param>
    public bool SaveLoginToken(string sessionTicket, string entityId) {
        string playfabid = PlayfabUtilities.GetPlayfabIdFromSessionTicket(sessionTicket);
        
        if (playfabid != "") {
            PlayerPrefs.SetString("Session_Ticket", sessionTicket);
            PlayerPrefs.SetString("Entity_Id", entityId);
            PlayerPrefs.SetString("Playfab_Id", playfabid);
            return true;
        }

        return false;

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
            bool temp = SaveLoginToken(result.SessionTicket,result.EntityToken.Entity.Id);
            print(temp);
            if (temp) {
                if (!loginCancelled) {
                    onPlayfabLoginSuccess?.Invoke();
                }
                
            }
            else {
                onPlayfabLoginFailed?.Invoke(new PlayFabError{Error = PlayFabErrorCode.ConnectionError});
            }
            
        }, error => {
            onPlayfabLoginFailed?.Invoke(error);
        });
    }
    /// <summary>
    /// Open info panel and Set the info message of the info panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="parameters">Parameters of the localized message, if exists</param>
    public void OpenInfoPanel(string localizedMessageId, bool addWaitingPeriod = false, params object[] parameters)
    {
        infoPanel.gameObject.SetActive(true);
        infoPanel.SetInfo(localizedMessageId,addWaitingPeriod,parameters);
    }

    /// <summary>
    /// Open info panel and Set the info message of the info panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    public void OpenInfoPanel(string localizedMessageId,bool addWaitingPeriod = false)
    {
        infoPanel.gameObject.SetActive(true);
        infoPanel.SetInfo(localizedMessageId,addWaitingPeriod);
    }
    /// <summary>
    /// Close Info Panel
    /// </summary>
    public void CloseInfoPanel() {
        infoPanel.gameObject.SetActive(false);
    }
}
