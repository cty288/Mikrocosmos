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
    public static string GetPlayfabIdFromSessionTicket(string sessionTicket) {
        string returned_result = "";
        bool waiting = true;
        
        PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
        {
            SessionTicket = sessionTicket
        }, result => {
            waiting = false;
            Debug.Log("got");
            returned_result = result.UserInfo.PlayFabId;
        }, error => {
            waiting = false;
            Debug.Log(error.Error.ToString());
            returned_result = "";
        });

        while (waiting) {

        }

        return returned_result;
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
    /// Return the username of the current playfab player
    /// </summary>
    /// <returns></returns>
    public static string GetUsername() {
        string username = "";
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest
        {
            PlayFabId = GetPlayFabIdFromPlayerPrefs()
        }, result => {
            username = result.AccountInfo.Username;
        }, error => {

        });
        return username;
    }


}
