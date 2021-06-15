using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : MonoBehaviour {
    
    [SerializeField] 
    private VerifiedInputBox[] items;

    private TMP_InputField emailInputField;
    private TMP_InputField userNameInputField;
    private TMP_InputField passwordInputField;




    private bool success = true;

    private Button registerButton;

    private void Awake() {
        registerButton = GetComponentInChildren<Button>();
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
    }

    void Start() {
        foreach (VerifiedInputBox inputBox in items)
        {
            switch (inputBox.InputBoxType) {
                case InputBoxType.Email:
                    emailInputField = inputBox.GetComponentInChildren<TMP_InputField>();
                    break;
                case InputBoxType.Username:
                    userNameInputField = inputBox.GetComponentInChildren<TMP_InputField>();
                    break;
                case InputBoxType.Password:
                    passwordInputField = inputBox.GetComponentInChildren<TMP_InputField>();
                    break;
            }
        }
    }
    void Update() {
        success = true;
        foreach (VerifiedInputBox inputBox in items) {
            if (inputBox.HasError) {
                success = false;
            }
        }

        registerButton.interactable = success;

    }

    private void OnRegisterButtonClicked() {
        Launcher._instance.OpenInfoPanel("LAUNCHER_WAIT_LOGIN", true);

        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Username = userNameInputField.text,
            Email = emailInputField.text,
            Password = passwordInputField.text
        }, result => {
            Launcher._instance.CloseInfoPanel();
            Launcher._instance.SaveLoginToken(result.SessionTicket, result.EntityToken.Entity.Id);
            Debug.Log("Login Success");
        }, error => {
            Launcher._instance.CloseInfoPanel();
            Debug.Log(error.GenerateErrorReport());
        });
    }
}
