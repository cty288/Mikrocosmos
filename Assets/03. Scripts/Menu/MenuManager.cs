using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class MenuManager : RootPanel {
    public static MenuManager _instance;

    void Awake() {
        _instance = this;
        Screen.SetResolution(1920, 1080, true);
        Screen.fullScreen = true;
    }

    void Start()
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest {
            PlayFabId = PlayfabTokenPasser._instance.Token.PlayfabId
        }, result => {
            print(result.PlayerProfile.DisplayName);
        }, error => {
            print(error.Error.ToString());
        });
    }


    void Update()
    {
        
    }
}
