using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MikrocosmosNewDatabase;
using MikroFramework.DatabaseKit.NHibernate;
using MikroFramework.Singletons;
using UnityEngine;

public class ServerDatabaseManager : MonoMikroSingleton<ServerDatabaseManager> {
   


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
         User userResult= await NHibernateTableManager<User>.Singleton.
             AuthenticateUsernamePlayfabid(token.Username, token.PlayfabId, token.Password);
         bool result;
         //await Task.Delay(2000);
         if (userResult != null) {
            Debug.Log($"[ServerDatabaseManager] {token.Username} authenticate success on the database! Authenticating display name...");
            result = await NHibernateTableManager<Player>.Singleton.AuthenticateDisplayName(userResult, token.PlayerName);
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


   
       
}

