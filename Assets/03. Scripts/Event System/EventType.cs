using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    LAUNCHER_OnPlayFabLoginSuccess,
    LAUNCHER_OnPlayFabLoginFailed,
    LAUNCHER_OnLoginPanelLoginSuccess,
    LAUNCHER_Error_Message,

    INTERNET_OnInternetLostConnection,
    INTERNET_OnInternetConnectionRecover,
}
