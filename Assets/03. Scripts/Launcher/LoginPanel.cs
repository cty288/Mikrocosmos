using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour {
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

        EventCenter.AddListener(EventType.LAUNCHER_OnPlayFabLoginSuccess,HandleOnLoginSuccess);
        EventCenter.AddListener<PlayFabError>(EventType.LAUNCHER_OnPlayFabLoginFailed,HandleOnLoginFailed);

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

    void OnDestroy() {
        EventCenter.RemoveListener(EventType.LAUNCHER_OnPlayFabLoginSuccess, HandleOnLoginSuccess);
        EventCenter.RemoveListener<PlayFabError>(EventType.LAUNCHER_OnPlayFabLoginFailed, HandleOnLoginFailed);
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

    private void HandleOnLoginSuccess() {
        

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
                EventCenter.Broadcast(EventType.LAUNCHER_OnLoginPanelLoginSuccess);
            }, error => {
                EventCenter.Broadcast(EventType.LAUNCHER_OnLoginPanelLoginSuccess);
            });

        }
        else {
            PlayerPrefs.SetInt("Remember_Account", 0);
            PlayerPrefs.SetString("Username_saved", "");
            PlayerPrefs.SetString("Password_saved","");
            EventCenter.Broadcast(EventType.LAUNCHER_OnLoginPanelLoginSuccess);
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

    private void HandleOnLoginFailed(PlayFabError error)
    {
        Launcher._instance.CloseInfoPanel();

        print(error.ErrorMessage);
        if (error.Error == PlayFabErrorCode.InvalidUsernameOrPassword) {
            EventCenter.Broadcast<string>(EventType.LAUNCHER_Error_Message, "LAUNCHER_LOGIN_FAILED_PWD");
        }
        else
        {
            EventCenter.Broadcast<string>(EventType.LAUNCHER_Error_Message, "LAUNCHER_LOGIN_NETWORK");
        }
 
    }


}
