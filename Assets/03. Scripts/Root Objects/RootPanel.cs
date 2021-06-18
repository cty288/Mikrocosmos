using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RootPanel : MonoBehaviour
{
    [SerializeField]
    protected ErrorPanel errorPanel;

    [SerializeField] protected InfoPanel infoPanel;

    void Start()
    {
        EventCenter.AddListener(EventType.INTERNET_OnInternetConnectionRecover, HandleOnInternetRecovered);
        EventCenter.AddListener(EventType.INTERNET_OnInternetLostConnection, HandleOnInternetLost);
    }

    void OnDestroy() {
        EventCenter.RemoveListener(EventType.INTERNET_OnInternetConnectionRecover, HandleOnInternetRecovered);
        EventCenter.RemoveListener(EventType.INTERNET_OnInternetLostConnection, HandleOnInternetLost);
    }

    protected virtual void HandleOnInternetRecovered() {
    }

    protected virtual void HandleOnInternetLost() {
    }

    /// <summary>
    /// Set the error message of the error panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="parameters">Parameters of the localized message, if exists</param>
    public void SetErrorMessage(string localizedMessageId, params object[] parameters)
    {
        errorPanel.gameObject.SetActive(true);
        errorPanel.SetErrorMessage(localizedMessageId, parameters);
    }

    /// <summary>
    /// Set the error message of the error panel to a specific localized message
    /// Recommend to trigger "LAUNCHER_Error_Message" event instead of directly call this method
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    public void SetErrorMessage(string localizedMessageId)
    {
        errorPanel.gameObject.SetActive(true);
        errorPanel.SetErrorMessage(localizedMessageId);
    }

    /// <summary>
    /// Open info panel and Set the info message of the info panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="parameters">Parameters of the localized message, if exists</param>
    public void OpenInfoPanel(string localizedMessageId, bool addWaitingPeriod = false, params object[] parameters)
    {
        infoPanel.gameObject.SetActive(true);
        infoPanel.SetInfo(localizedMessageId, addWaitingPeriod, parameters);
    }

    /// <summary>
    /// Open info panel and Set the info message of the info panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    public void OpenInfoPanel(string localizedMessageId, bool addWaitingPeriod = false)
    {
        infoPanel.gameObject.SetActive(true);
        infoPanel.SetInfo(localizedMessageId, addWaitingPeriod);
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
        infoPanel.gameObject.SetActive(true);
        infoPanel.SetInfo(localizedMessageId, onCloseButtonClickAction, addWaitingPeriod);
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
        infoPanel.gameObject.SetActive(true);
        infoPanel.SetInfo(localizedMessageId, onCloseButtonClickAction, addWaitingPeriod, parameters);
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