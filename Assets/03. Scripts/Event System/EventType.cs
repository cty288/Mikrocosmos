using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    LAUNCHER_OnPlayFabLoginSuccess,
    LAUNCHER_OnPlayFabLoginFailed,
    LAUNCHER_OnLoginPanelLoginSuccess,
    LAUNCHER_Error_Message,

    INTERNET_OnInternetLostConnection,
    INTERNET_OnInternetConnectionRecover,
    
    MENU_AuthorityOnConnected,
    MENU_OnNewUserEnterMenu,
    MENU_OnUserEnterMenu,
    MENU_Error,
    MENU_MATCHMAKING_ClientRequestingMatchmaking,
    MENU_MATCHMAKING_ClientMatchmakingSuccess,
    MENU_MATCHMAKING_ClientMatchmakingFailed,
    MENU_MATCHMAKING_ClientMatchmakingCancelled,
    MENU_MATCHMAKING_ClientMatchmakingReadyToGet,
    MENU_OnServerPlayerAdded,
    MENU_OnServerPlayerDisconnected,

    MENU_OnClientLobbyInfoUpdated,
    MENU_OnClientLobbyStateUpdated,
    MENU_OnClientLeaveLobbyFailed,
    MENU_OnClientLeaveLobbySuccess,
    MENU_OnClientLobbyCountdownUpdated,

    MENU_OnServerMatchStartingProcess,
    MENU_OnClientReceiveServerStartingProcessFailed,


    GAME_OnClientConnectingToServer,
    GAME_OnClientConnectingToServerFailed,
    GAME_OnClientConnectingToServerSuccess,
    GAME_OnClientAuthenticated,

    GAME_ServerOnPlayerConnected,
    GAME_ServerOnPlayerDisconnected,
    GAME_OnMatchExited,

}
