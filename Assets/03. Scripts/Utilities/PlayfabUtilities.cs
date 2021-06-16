using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ServerModels;
using UnityEngine;

public static class PlayfabUtilities
{
    /// <summary>
    /// Return Playfabid from session ticket. return an empty string if failed to get the playfab id
    /// </summary>
    /// <param name="sessionTicket"></param>
    /// <returns></returns>
    public static void SetPlayfabIdFromSessionTicket(string sessionTicket) {
        string returned_result = "";
        
        PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
        {
            SessionTicket = sessionTicket
        }, result => {
            Debug.Log("got");
            returned_result = result.UserInfo.PlayFabId;
        }, error => {
            Debug.Log(error.Error.ToString());
            returned_result = "";
        });

        PlayerPrefs.SetString("Playfab_Id",returned_result);
    }

    /// <summary>
    /// return Playfab Id from local device. Only call this after the player logins/registers
    /// </summary>
    /// <returns></returns>
    public static string GetPlayFabIdFromPlayerPrefs() {
        return PlayerPrefs.GetString("Playfab_Id");
    }

    /// <summary>
    /// return session ticket from local device. Only call this after the player logins/registers
    /// </summary>
    /// <returns></returns>
    public static string GetSessionTicket() {
        return PlayerPrefs.GetString("Session_Ticket");
    }
    /// <summary>
    ///  return entity id from local device. Only call this after the player logins/registers
    /// </summary>
    /// <returns></returns>
    public static string GetEntityId() {
        return PlayerPrefs.GetString("Entity_Id");
    }

    /// <summary>
    /// Get the username based playfabid. Must be called after login/register
    /// </summary>
    /// <param name="onUserNameGet">Function called after name successfully get</param>
    /// <param name="onError">Function called when error occurs</param>
    public static void GetUsername(Action<string> onUserNameGet, Action<PlayFabError> onError) {
        string username = "";
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest
        {
            PlayFabId = GetPlayFabIdFromPlayerPrefs()
        }, result => {
            username = result.AccountInfo.Username;
            onUserNameGet?.Invoke(username);
        }, error => {
            onError?.Invoke(error);
        });
    }


}
