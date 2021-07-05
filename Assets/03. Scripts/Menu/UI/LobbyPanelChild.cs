using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPanelChild : MonoBehaviour {
    
    public static Action onLobbyPanelChildEnabled;
    public static Action onLobbyPanelChildDisabled;


    void OnEnable() {
        print("Lobby panel child enabled");
        LobbyPanelChild.onLobbyPanelChildEnabled?.Invoke();
    }

    void OnDisable() {
        LobbyPanelChild.onLobbyPanelChildDisabled?.Invoke();
    }
}
