using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;
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

        MenuManager._instance.UpdateAndSaveDisplayName(playerNameInputBox.InputField.text,
            (displayName) => {
                MenuManager._instance.StopWaiting();
                MenuManager._instance.OpenInfoPanel("MENU_USER_NAME_CREATE_SUCCESS",
                    () => {
                        EventCenter.Broadcast(EventType.MENU_OnUserEnterMenu);
                        this.gameObject.SetActive(false);
                    });
            }, error => {
                MenuManager._instance.StopWaiting();
                if (error.Error == PlayFabErrorCode.NameNotAvailable) {
                    EventCenter.Broadcast<string, UnityAction, string, object[]>(EventType.MENU_Error,
                        "MENU_USER_NAME_RULE_ERROR2", () => { }, "MENU_RETRY", null);
                }
                else if (error.Error == PlayFabErrorCode.ProfaneDisplayName) {
                    EventCenter.Broadcast<string, UnityAction, string, object[]>(EventType.MENU_Error,
                        "MENU_USER_NAME_RULE_ERROR3", () => { }, "MENU_RETRY", null);
                }
                else {
                    EventCenter.Broadcast<string, UnityAction, string, object[]>(EventType.MENU_Error,
                        "MENU_UNKNOWN_ERROR", () => { }, "MENU_RETRY", null);
                }
            });

    }



}
