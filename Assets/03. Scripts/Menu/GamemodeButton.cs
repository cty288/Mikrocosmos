using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamemodeButton : MonoBehaviour {
    [SerializeField] 
    private Mode gamemode;

    void Awake() {
        GetComponent<Button>().onClick.AddListener(OnGameModeButtonClicked);
    }
    private void OnGameModeButtonClicked()
    {
        MenuManager._instance.RequestMatch(gamemode);
    }
}
