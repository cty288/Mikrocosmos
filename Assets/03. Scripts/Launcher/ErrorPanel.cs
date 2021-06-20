using System;
using System.Collections;
using System.Collections.Generic;
using Polyglot;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ErrorPanel : MonoBehaviour {
    [SerializeField] 
    private Text errorMessage;

    [SerializeField] private Text buttonText;

    [SerializeField] private Button closeButton;

    /// <summary>
    /// Set the error message of the error panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="parameters">Parameters of the localized message, if exists</param>
    public void SetErrorMessage(string localizedMessageId,params object[] parameters) {
        ResetButtonListener();
        errorMessage.text = Localization.GetFormat(localizedMessageId,parameters);
    }

    /// <summary>
    /// Set the error message of the error panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    public void SetErrorMessage(string localizedMessageId)
    {
        ResetButtonListener();
        errorMessage.text = Localization.Get(localizedMessageId);
    }
    /// <summary>
    /// Set the error message of the error panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="onCloseButtonClicked">Event Triggered when the close button is clicked</param>
    ///<param name="localizedButtonText">Localized text shown on the close button</param>
    public void SetErrorMessage(string localizedMessageId, UnityAction onCloseButtonClicked, 
        string localizedButtonText) {
        ResetButtonListener();
        closeButton.onClick.AddListener(onCloseButtonClicked);
        errorMessage.text = Localization.Get(localizedMessageId);
        buttonText.text = Localization.Get(localizedButtonText);
    }

    /// <summary>
    /// Set the error message of the error panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="parameters">Parameters of the localized message, if exists</param>
    /// <param name="localizedButtonText">Localized text shown on the close button</param>
    /// <param name="onCloseButtonClicked">Event Triggered when the close button is clicked</param>
    public void SetErrorMessage(string localizedMessageId, UnityAction oncloseButtonClicked, 
        string localizedButtonText , params object[] parameters)
    {
        ResetButtonListener();
        closeButton.onClick.AddListener(oncloseButtonClicked);
        errorMessage.text = Localization.GetFormat(localizedMessageId, parameters);
        buttonText.text = Localization.Get(localizedButtonText);
    }

    private void ResetButtonListener() {
        if (closeButton) {
            closeButton.onClick.RemoveAllListeners();
            buttonText.text = Localization.Get("MENU_LABEL_CLOSE");
        }
    }

}
