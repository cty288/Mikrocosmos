using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Event;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using EventType = MikroFramework.Event.EventType;

public class FirstTimeUserPanel : MikroBehavior {
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
                        Broadcast(EventType.MENU_OnUserEnterMenu,null);
                        this.gameObject.SetActive(false);
                    });
            }, error => {
                MenuManager._instance.StopWaiting();
                if (error.Error == PlayFabErrorCode.NameNotAvailable) {
                    Broadcast(EventType.MENU_Error, MikroMessage.Create(
                        "MENU_USER_NAME_RULE_ERROR2", (Action)(() => { }), "MENU_RETRY", null));
                }
                else if (error.Error == PlayFabErrorCode.ProfaneDisplayName) {
                    Broadcast(EventType.MENU_Error,
                        MikroMessage.Create("MENU_USER_NAME_RULE_ERROR3",  (Action) (() => { }), "MENU_RETRY", null));
                }
                else {
                    Broadcast(EventType.MENU_Error,
                        MikroMessage.Create("MENU_UNKNOWN_ERROR", (Action)(() => { }), "MENU_RETRY", null));
                }
            });

    }

    protected override void OnBeforeDestroy() {
        
    }
}
