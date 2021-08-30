using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Event;
using PlayFab;
using PlayFab.ClientModels;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = MikroFramework.Event.EventType;

public class LoginPanel : MikroBehavior {
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    [SerializeField] private Toggle rememberAccountToggle;
    [SerializeField] private Toggle autoLoginToggle;

    private Button loginButton;
    private Button forgotPwdButton;

    void Awake() {
        loginButton = transform.Find("Login Button").GetComponent<Button>();
        forgotPwdButton = transform.Find("Forgot Password Button").GetComponent<Button>();
    }
    void Start() {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        autoLoginToggle.onValueChanged.AddListener(onAutoLoginToggleTurnOff);
        rememberAccountToggle.onValueChanged.AddListener(onRememberAccountToggleTurnOff);

        AddListener(EventType.LAUNCHER_OnPlayFabLoginSuccess,HandleOnLoginSuccess);
        AddListener(EventType.LAUNCHER_OnPlayFabLoginFailed,HandleOnLoginFailed);

        InitializeInputFieldAndAutoLogin();
    }

    private void onAutoLoginToggleTurnOff(bool isOn)
    {
        if (!isOn) {
            PlayerPrefs.SetInt("Auto_Login", 0);
        }
    }

    private void onRememberAccountToggleTurnOff(bool isOn) {
        if (!isOn && !autoLoginToggle.isOn) {
            PlayerPrefs.SetInt("Remember_Account",0);
        }
    }


    protected override void OnBeforeDestroy() {
        
    }

    void Update()
    {
        if (autoLoginToggle.isOn) {
            rememberAccountToggle.isOn = true;
        }
    }

    private void OnLoginButtonClicked() {
        Launcher._instance.Login(usernameInputField.text,passwordInputField.text);
        Launcher._instance.OpenInfoPanel("LAUNCHER_WAIT_LOGIN",Launcher._instance.CancelLogin
            ,true);
    }

    private void HandleOnLoginSuccess(MikroMessage msg) {
        

        if (autoLoginToggle.isOn)
        {
            PlayerPrefs.SetInt("Auto_Login", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Auto_Login", 0);
        }

        if (rememberAccountToggle.isOn) {
            PlayerPrefs.SetInt("Remember_Account",1);
            PlayerPrefs.SetString("Password_saved", passwordInputField.text);


            PlayfabUtilities.GetUsername(username => {
                PlayerPrefs.SetString("Username_saved",username);
                Broadcast(EventType.LAUNCHER_OnLoginPanelLoginSuccess,null);
            }, error => {
                Broadcast(EventType.LAUNCHER_OnLoginPanelLoginSuccess, null);
            });

        }
        else {
            PlayerPrefs.SetInt("Remember_Account", 0);
            PlayerPrefs.SetString("Username_saved", "");
            PlayerPrefs.SetString("Password_saved","");
            Broadcast(EventType.LAUNCHER_OnLoginPanelLoginSuccess,null);
        }


    }




    private void InitializeInputFieldAndAutoLogin() {
        if (PlayerPrefs.GetInt("Remember_Account", 0) == 1) { //remember account
            rememberAccountToggle.isOn = true;
            usernameInputField.text = PlayerPrefs.GetString("Username_saved");
            passwordInputField.text = PlayerPrefs.GetString("Password_saved");
        }
        else {
            rememberAccountToggle.isOn = false;
            usernameInputField.text = "";
            passwordInputField.text = "";
        }

        if (PlayerPrefs.GetInt("Auto_Login", 0) == 1) { //auto login
            autoLoginToggle.isOn = true;
            OnLoginButtonClicked();
        }
    }

    private void HandleOnLoginFailed(MikroMessage msg)
    {
        PlayFabError error = msg.GetSingleMessage() as PlayFabError;
        
        Launcher._instance.CloseInfoPanel();

        print(error.ErrorMessage);
        if (error.Error == PlayFabErrorCode.InvalidUsernameOrPassword) {
            Broadcast(EventType.LAUNCHER_Error_Message, MikroMessage.Create("LAUNCHER_LOGIN_FAILED_PWD"));
        }
        else
        {
            Broadcast(EventType.LAUNCHER_Error_Message, MikroMessage.Create("LAUNCHER_LOGIN_NETWORK"));
        }
 
    }


}
