using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ServerModels;
using UnityEngine;
using UnityEngine.Events;
using GetPlayerProfileRequest = PlayFab.ClientModels.GetPlayerProfileRequest;

public static class PlayfabUtilities
{
    /// <summary>
    /// Return Playfabid from session ticket. return an empty string if failed to get the playfab id
    /// </summary>
    /// <param name="sessionTicket"></param>
    /// <returns></returns>
    public static void SetPlayfabIdFromSessionTicket(string sessionTicket,
        Action onSuccessfullySet,Action onSetFailed) {
        string returned_result = "";
        
        PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
        {
            SessionTicket = sessionTicket
        }, result => {
            Debug.Log("got");
            returned_result = result.UserInfo.PlayFabId;

            PlayerPrefs.SetString("Playfab_Id", returned_result);
            onSuccessfullySet?.Invoke();
        }, error => {
            Debug.Log(error.Error.ToString());
            returned_result = "";
            PlayerPrefs.SetString("Playfab_Id", returned_result);
            onSetFailed?.Invoke();

        });

       
    }

    /// <summary>
    /// return Playfab Id from local device. Only call this after the player logins/registers
    /// </summary>
    /// <returns></returns>
    public static string GetPlayFabIdFromPlayerPrefs() {
        return PlayerPrefs.GetString("Playfab_Id");
    }

    /// <summary>
    /// return session ticket from token passer. Only call this after the player logins/registers
    /// </summary>
    /// <returns></returns>
    public static string GetSessionTicket() {
        return PlayfabTokenPasser._instance.Token.SessionTicket;
    }
    /// <summary>
    ///  return entity id from token passer. Only call this after the player logins/registers
    /// </summary>
    /// <returns></returns>
    public static string GetEntityId() {
        return PlayfabTokenPasser._instance.Token.EntityId;
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

    public static string GetUsernameFromPlayPrefs() {
        return PlayerPrefs.GetString("Username");
    }

    /// <summary>
    /// Get Player name. Must be called after login/register and successfully setup a display name
    /// </summary>
    /// <returns></returns>
    public static string GetPlayerName() {
        return PlayfabTokenPasser._instance.Token.PlayerName;
    }

    /// <summary>
    /// Update the player's display name to Playfab Server. Must be called after login
    /// </summary>
    /// <param name="onSuccess">Event triggered after successfully set name</param>
    /// <param name="onFailed">Event triggered after set failed</param>
    public static void UpdateDisplayName(string newName, Action<string> onSuccess, Action<PlayFabError> onFailed) {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {
            DisplayName = newName
        }, result => {
            onSuccess?.Invoke(newName);
        }, error => {
            onFailed?.Invoke(error);
        });
    }

    /// <summary>
    /// Get Title ID of the game from PlayFab Server
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onFailed"></param>
    public static void GetTitleID(Action<string> onSuccess,Action onFailed) {
        PlayFabServerAPI.GetTitleData(new PlayFab.ServerModels.GetTitleDataRequest
        {

        }, result => {
            if (result.Data != null & result.Data.ContainsKey("TitleID"))
            {
                Debug.Log(result.Data["TitleID"]);
                onSuccess?.Invoke(result.Data["TitleID"]);
            }
        }, error => {
            Debug.Log(error.Error.ToString());
            onFailed?.Invoke(); 
        });
    }

    


}
