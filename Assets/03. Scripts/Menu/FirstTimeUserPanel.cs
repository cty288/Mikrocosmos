using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

public class FirstTimeUserPanel : MonoBehaviour {
    private VerifiedInputBox playerNameInputBox;
    private Button confirmButton;

    void Awake() {
        confirmButton = GetComponentInChildren<Button>();
        playerNameInputBox = GetComponentInChildren<VerifiedInputBox>();
    }

    void Start() {
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
    }


    void Update() {
        confirmButton.interactable = !playerNameInputBox.HasError;
    }

    private void OnConfirmButtonClicked() {
        MenuManager._instance.StartWaiting();
        PlayfabUtilities.UpdateDisplayName(playerNameInputBox.InputField.text,
            (displayName) => {
                MenuManager._instance.StopWaiting();
                PlayfabTokenPasser._instance.SavePlayerName(displayName);
                MenuManager._instance.OpenInfoPanel("MENU_USER_NAME_CREATE_SUCCESS",
                    () => {
                        this.gameObject.SetActive(false);
                    });
            }, error => {
                MenuManager._instance.StopWaiting();
                if (error.Error == PlayFabErrorCode.NameNotAvailable) {
                    MenuManager._instance.SetErrorMessage("MENU_USER_NAME_RULE_ERROR2");
                }else if (error.Error == PlayFabErrorCode.ProfaneDisplayName) {
                    MenuManager._instance.SetErrorMessage("MENU_USER_NAME_RULE_ERROR3");
                }
                else {
                    MenuManager._instance.SetErrorMessage("MENU_UNKNOWN_ERROR");
                }
            });
    }


}
