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

    public void SetAvatar(Sprite sprite) {
        avatarImage.sprite = sprite;
    }

    public void SetDisplayName(string displayName) {
        displayNameText.text = displayName;
    }

    public string GetDisplayName() {
        return displayNameText.text;
    }
}
