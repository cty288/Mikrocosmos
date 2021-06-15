using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour {
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    private Button loginButton;
    private Button forgotPwdButton;

    void Awake() {
        loginButton = transform.Find("Login Button").GetComponent<Button>();
        forgotPwdButton = transform.Find("Forgot Password Button").GetComponent<Button>();
    }

    void Start() {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        Launcher._instance.onPlayfabLoginSuccess += HandleOnLoginSuccess;
        Launcher._instance.onPlayfabLoginFailed += HandleOnLoginFailed;
    }


    void OnDestroy() {
        Launcher._instance.onPlayfabLoginSuccess -= HandleOnLoginSuccess;
        Launcher._instance.onPlayfabLoginFailed -= HandleOnLoginFailed;
    }
    void Update()
    {
        
    }

    private void OnLoginButtonClicked() {
        Launcher._instance.Login(usernameInputField.text,passwordInputField.text);
        Launcher._instance.OpenInfoPanel("LAUNCHER_WAIT_LOGIN",true);
    }

    private void HandleOnLoginSuccess() {
        Launcher._instance.CloseInfoPanel();
    }

    private void HandleOnLoginFailed(PlayFabError error)
    {
        Launcher._instance.CloseInfoPanel();
        Launcher._instance.SetErrorMessage("LAUNCHER_LOGIN_FAILED");
    }

}
