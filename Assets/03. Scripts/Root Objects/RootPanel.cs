using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Event;
using Polyglot;
using UnityEngine;
using UnityEngine.Events;
using EventType = MikroFramework.Event.EventType;

public class RootPanel : MikroBehavior
{
    [SerializeField]
    protected ErrorPanel errorPanel;

    [SerializeField] protected InfoPanel infoPanel;

    void Start()
    {
        AddListener(EventType.INTERNET_OnInternetConnectionRecover, HandleOnInternetRecovered);
        AddListener(EventType.INTERNET_OnInternetLostConnection, HandleOnInternetLost);
    }

    

    protected override void OnBeforeDestroy() { }

    protected virtual void HandleOnInternetRecovered(MikroMessage msg) {
    }

    protected virtual void HandleOnInternetLost(MikroMessage msg) {
    }

    /// <summary>
    /// Set the error message of the error panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="parameters">Parameters of the localized message, if exists</param>
    public void SetErrorMessage(string localizedMessageId, params object[] parameters)
    {
        if (errorPanel)
        {
            errorPanel.gameObject.SetActive(true);
            errorPanel.SetErrorMessage(localizedMessageId, parameters);
        }

    }

    /// <summary>
    /// Set the error message of the error panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="onCloseButtonClicked">Event Triggered when the close button is clicked</param>
    ///<param name="localizedButtonText">Localized text shown on the close button</param>
    public void SetErrorMessage(string localizedMessageId,UnityAction onCloseButtonClicked,string localizedButtonText)
    {
        if (errorPanel) {
            errorPanel.gameObject.SetActive(true);
            errorPanel.SetErrorMessage(localizedMessageId, onCloseButtonClicked, localizedButtonText);
        }
        
    }

    /// <summary>
    /// Set the error message of the error panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="parameters">Parameters of the localized message, if exists</param>
    /// <param name="localizedButtonText">Localized text shown on the close button</param>
    /// <param name="onCloseButtonClicked">Event Triggered when the close button is clicked</param>
    public void SetErrorMessage(string localizedMessageId, UnityAction oncloseButtonClicked,
        string localizedButtonText, params object[] parameters)
    {
        if (errorPanel) {
            errorPanel.gameObject.SetActive(true);
            errorPanel.SetErrorMessage(localizedMessageId, oncloseButtonClicked, localizedButtonText, parameters);
        }

    }


    /// <summary>
    /// Set the error message of the error panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    public void SetErrorMessage(MikroMessage msg)
    {
        if (errorPanel) {
            errorPanel.gameObject.SetActive(true);
            errorPanel.SetErrorMessage(msg.GetSingleMessage().ToString());
        }
    }

    /// <summary>
    /// Open info panel and Set the info message of the info panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="parameters">Parameters of the localized message, if exists</param>
    public void OpenInfoPanel(string localizedMessageId, bool addWaitingPeriod = false, params object[] parameters)
    {
        if (infoPanel)
        {
            infoPanel.gameObject.SetActive(true);
            infoPanel.SetInfo(localizedMessageId, addWaitingPeriod, parameters);
        }

    }

    /// <summary>
    /// Open info panel and Set the info message of the info panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    public void OpenInfoPanel(string localizedMessageId, bool addWaitingPeriod = false)
    {
        if (infoPanel)
        {
            infoPanel.gameObject.SetActive(true);
            infoPanel.SetInfo(localizedMessageId, addWaitingPeriod);
        }

    }
    /// <summary>
    /// Open info panel and Set the info message of the info panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="onCloseButtonClickAction">Event triggered when the user clicks the close button (other than close the panel)</param>
    /// <param name="addWaitingPeriod"></param>
    public void OpenInfoPanel(string localizedMessageId, UnityAction onCloseButtonClickAction,
        bool addWaitingPeriod = false)
    {
        if (infoPanel)
        {
            infoPanel.gameObject.SetActive(true);
            infoPanel.SetInfo(localizedMessageId, onCloseButtonClickAction, addWaitingPeriod);
        }

    }

    /// <summary>
    /// Open info panel and Set the info message of the info panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="onCloseButtonClickAction">Event triggered when the user clicks the close button (other than close the panel)</param>
    /// <param name="addWaitingPeriod"></param>
    /// <param name="parameters"></param>
    public void OpenInfoPanel(string localizedMessageId, UnityAction onCloseButtonClickAction,
        bool addWaitingPeriod = false, params object[] parameters)
    {
        if (infoPanel)
        {
            infoPanel.gameObject.SetActive(true);
            infoPanel.SetInfo(localizedMessageId, onCloseButtonClickAction, addWaitingPeriod, parameters);
        }

    }

    /// <summary>
    /// Close Info Panel
    /// </summary>
    public void CloseInfoPanel()
    {
        if (infoPanel)
        {
            infoPanel.gameObject.SetActive(false);
        }
    }

}
