using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour {
    [SerializeField]
    private Image avatarImage;
    [SerializeField] 
    private TMP_Text displayNameText;

    private PlayerTeamInfo playerInfo;

    public void SetAvatar(Sprite sprite) {
        avatarImage.sprite = sprite;
    }


    public void SetPlayerTeamInfo(PlayerTeamInfo teamInfo) {
        this.playerInfo = teamInfo;
        displayNameText.text = teamInfo.DisplayName;
    }

    public PlayerTeamInfo GetPlayerTeamInfo() {
        return this.playerInfo;
    }


}
