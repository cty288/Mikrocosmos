using System;
using System.Collections;
using System.Collections.Generic;
using Polyglot;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {
    [SerializeField] private Text infoText;

    private string mainInfo = "";
    private string periodString = "";
    private bool hasWaitingPeriod = false;

    [SerializeField] private float waitingPeriodUpdateTimeInterval = 0.3f;
    private float timer = 0;
    private const int periodMaxNumber=3;
    private int currentPeriodNumber = 0;
    [SerializeField]
    private bool hasCloseButton;
    private UnityAction onCloseEvent;
    [SerializeField]
    private Button closeButton;


    void Start() {
        closeButton.onClick.AddListener(onCloseEvent);
        SetInfo();
    }

    void Update() {
        closeButton.gameObject.SetActive(hasCloseButton);
        if (hasWaitingPeriod) {
            timer+=Time.deltaTime;
            if (timer >= waitingPeriodUpdateTimeInterval) {
                timer = 0;
                currentPeriodNumber++;
                currentPeriodNumber %= periodMaxNumber + 1;
            }

            switch (currentPeriodNumber) {
                case 0:
                    periodString = "";
                    break;
                case 1:
                    periodString = ".";
                    break;
                case 2:
                    periodString = "..";
                    break;
                case 3:
                    periodString = "...";
                    break;
            }

            infoText.text = mainInfo + periodString;
        }
        else {
            infoText.text = mainInfo;
        }
    }

    private void SetInfo() {
        onCloseEvent = null;
        onCloseEvent += () => { this.gameObject.SetActive(false); };

        closeButton.interactable = true;
    }

    /// <summary>
    /// Set the info message of the info panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="parameters">Parameters of the localized message, if exists</param>
    public void SetInfo(string localizedMessageId,bool addWaitingPeriod=false, params object[] parameters)
    {
        SetInfo();
        hasCloseButton = false;
        mainInfo = Localization.GetFormat(localizedMessageId, parameters);
        hasWaitingPeriod = addWaitingPeriod;
    }

    /// <summary>
    /// Set the info message of the info panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    public void SetInfo(string localizedMessageId, bool addWaitingPeriod = false)
    {
        SetInfo();
        hasCloseButton = false;
        mainInfo = Localization.Get(localizedMessageId);
        hasWaitingPeriod = addWaitingPeriod;
    }

    /// <summary>
    /// Set the info message of the info panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="onCloseButtonClickAction">Event triggered when the user clicks the close button (other than close the panel)</param>
    /// <param name="addWaitingPeriod"></param>
    public void SetInfo(string localizedMessageId, UnityAction onCloseButtonClickAction, 
        bool addWaitingPeriod = false) {
        SetInfo();
        hasCloseButton = true;
        onCloseEvent += onCloseButtonClickAction;
        mainInfo = Localization.Get(localizedMessageId);
        hasWaitingPeriod = addWaitingPeriod;
    }


    /// <summary>
    /// Set the info message of the info panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="onCloseButtonClickAction">Event triggered when the user clicks the close button (other than close the panel)</param>
    /// <param name="addWaitingPeriod"></param>
    /// <param name="parameters"></param>
    public void SetInfo(string localizedMessageId, UnityAction onCloseButtonClickAction,
        bool addWaitingPeriod = false, params object[] parameters)
    {
        SetInfo();
        hasCloseButton = true;
        onCloseEvent += onCloseButtonClickAction;
        mainInfo = Localization.GetFormat(localizedMessageId, parameters);
        hasWaitingPeriod = addWaitingPeriod;
    }

    public void DisableCloseButton() {
        closeButton.interactable = false;
    }
}
