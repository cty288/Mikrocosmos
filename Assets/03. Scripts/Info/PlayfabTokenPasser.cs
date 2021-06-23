using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfabTokenPasser : MonoBehaviour {
    public static PlayfabTokenPasser _instance;

    private PlayfabToken token;
    public PlayfabToken Token => token;

    void Awake() {
        _instance = this;
        token = new PlayfabToken();
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Save PlayFab tokens to the token property of the PlayfabTokenPasser
    /// </summary>
    /// <param name="sessionTicket"></param>
    /// <param name="entityId"></param>
    /// <param name="playfabId"></param>
    public void SaveToken(string sessionTicket, string entityId, string playfabId) {
        token.SessionTicket = sessionTicket;
        token.EntityId = entityId;
        token.PlayfabId = playfabId;
    }

    public void SavePlayerName(string playerName) {
        token.PlayerName = playerName;
    }
}

/// <summary>
/// This struct saves session ticket, entity id, and playfab id
/// </summary>
public struct PlayfabToken {
    public string SessionTicket, EntityId, PlayfabId;
    public string PlayerName;

    public PlayfabToken(string SessionTicket, string EntityId,string PlayfabId, string PlayerName) {
        this.SessionTicket = SessionTicket;
        this.EntityId = EntityId;
        this.PlayerName = PlayerName;
        this.PlayfabId = PlayfabId;
    }
}
