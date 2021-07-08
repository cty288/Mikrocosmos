using System;
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
    /// This method will run asynchronously
    /// </summary>
    /// <param name="token"></param>
    /// <param name="onAuthenticateSuccess">Event triggered after authenticate success</param>
    /// <param name="onAuthenticateFailed">Event triggered after authenticate failed</param>
    /// <returns></returns>
    public async void AuthenticatePlayfabToken(PlayfabToken token,Action onAuthenticateSuccess,Action onAuthenticateFailed) {
         User userResult= await userTableManager.AuthenticateUsernamePlayfabid(token.Username, token.PlayfabId, token.Password);
         bool result;
         //await Task.Delay(2000);
         if (userResult != null) {
            Debug.Log($"[ServerDatabaseManager] {token.Username} authenticate success on the database! Authenticating display name...");
            result = await playerTableManager.AuthenticateDisplayName(userResult, token.PlayerName);
         }else {
            Debug.Log($"[ServerDatabaseManager] {token.Username} authenticate failed on the database!");
            result = false;
         }

        if (result) {
            onAuthenticateSuccess?.Invoke();
        }
        else {
            onAuthenticateFailed?.Invoke();
        }
    }


    public async void AddMatchIdToDatabase(PlayerTeamInfo teamInfo, string matchId) {
        string displayName = teamInfo.DisplayName;
        Player searchedPlayer = await playerTableManager.SearchByDisplayName(displayName);

        if (searchedPlayer != null) {
            searchedPlayer.JoinedMatchid = matchId;

            bool result = await playerTableManager.Update(searchedPlayer);

            if (result) 
                Debug.Log($"[ServerDatabaseManager] Successfully updated {matchId} to {displayName}'s data on the database!");
            }else {
                Debug.Log($"[ServerDatabaseManager] Failed to update {matchId} to {displayName}'s data on the database!");
        }
    }
       
}

