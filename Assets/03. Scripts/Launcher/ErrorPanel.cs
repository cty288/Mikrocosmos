using System.Collections;
using System.Collections.Generic;
using Polyglot;
using UnityEngine;
using UnityEngine.UI;

public class ErrorPanel : MonoBehaviour {
    [SerializeField] 
    private Text errorMessage;

    /// <summary>
    /// Set the error message of the error panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    /// <param name="parameters">Parameters of the localized message, if exists</param>
    public void SetErrorMessage(string localizedMessageId,params object[] parameters) {
        errorMessage.text = Localization.GetFormat(localizedMessageId,parameters);
    }

    /// <summary>
    /// Set the error message of the error panel to a specific localized message
    /// </summary>
    /// <param name="localizedMessageId">The localized id of the message</param>
    public void SetErrorMessage(string localizedMessageId)
    {
        errorMessage.text = Localization.Get(localizedMessageId);
    }

}
