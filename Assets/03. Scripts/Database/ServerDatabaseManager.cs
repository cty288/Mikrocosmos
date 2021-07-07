using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MikrocosmosDatabase;
using UnityEngine;

public class ServerDatabaseManager : MonoBehaviour {
    private UserTableManager userTableManager;
    private PlayerTableManager playerTableManager;

    private static ServerDatabaseManager singleton;
    public static ServerDatabaseManager Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = new ServerDatabaseManager();
            }

            return singleton;
        }
    }


    void Awake() {
        singleton = this;
        userTableManager = new UserTableManager();
        playerTableManager = new PlayerTableManager();
    }

    /// <summary>
    /// Authenticate the PlayFabToken in the database. Create a new data in the database if it's a new user
    /// Return true if authenticate success, false if authenticate failed
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public bool AuthenticatePlayfabToken(PlayfabToken token) {
         User userResult= userTableManager.AuthenticateUsernamePlayfabid(token.Username, token.PlayfabId, token.Password);
        if (userResult != null) {
            Debug.Log($"{token.Username} authenticate success on the database! Authenticating display name...");
            return  playerTableManager.AuthenticateDisplayName(userResult, token.PlayerName);
        }
        else {
            Debug.Log($"{token.Username} authenticate failed on the database!");
            return false;
        }
    }
}
