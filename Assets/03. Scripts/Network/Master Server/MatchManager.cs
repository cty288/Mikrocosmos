using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public enum MatchError {
    UnableToFindMatch,
    MatchAlreadyStart,
    UnableToFindPlayer,
    NoError,
}
public class MatchManager : NetworkBehaviour {
    private List<GameMatch> unStartedMatchList;
    private List<GameMatch> startedMatchList;
    private ushort currentPort = 7778;
    private const ushort Max_Port = 60000;
    private const ushort Min_Port = 7778;
    
    [ServerCallback]
    void Awake() {
        unStartedMatchList = new List<GameMatch>();
        startedMatchList = new List<GameMatch>();
    }

    [ServerCallback]
    private void Start() {
        EventCenter.AddListener<GameMatch>(EventType.MENU_OnServerMatchStartingProcess,HandleOnMatchStartingProcess);
    }

    private void OnDestroy() {
        EventCenter.RemoveListener<GameMatch>(EventType.MENU_OnServerMatchStartingProcess, HandleOnMatchStartingProcess);
    }
    /// <summary>
    /// Return an available game match
    /// </summary>
    /// <param name="gamemode"></param>
    /// <returns></returns>
    [ServerCallback]
    public GameMatch FindAvailableMatch(GameMode gamemode) {
        foreach (GameMatch match in unStartedMatchList) {
            if (match.Gamemode.getGameMode() == gamemode.getGameMode()) {
                if (match.GetCurrentPlayerNumber() < match.GetRequiredPlayerNumber()) {
                    return match;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Create a new match room based on gamemode and matchId.
    /// </summary>
    /// <param name="gamemode"></param>
    /// <param name="matchId">Matchid. Use PlayFab to generate one</param>
    /// <returns>The requested GameMatch object. null if failed to create</returns>
    [ServerCallback]
    private GameMatch CreateMatchRoom(GameMode gamemode, string matchId) {
        //just create the room; no playfab matchmaking
        GameObject createdRoom = MirrorServerUtilities.SpawnServerOnlyObject<GameMatch>($"Gamematch: {matchId}");
        if (createdRoom != null) {
            GameMatch result = createdRoom.GetComponent<GameMatch>();
            result.SetGamemode(gamemode,matchId,currentPort);
            UpdatePort();
            unStartedMatchList.Add(result);
            Debug.Log($"Successfully created match room. ID: {matchId}");
            return result;
        }
        else {
            return null;
        }
    }

    /// <summary>
    /// Create a new match room based on generated playfab matchid and gamemode
    /// </summary>
    /// <param name="gamemode"></param>
    /// <param name="matchId"></param>
    /// <returns></returns>
    [ServerCallback]
    public GameMatch RequestNewPlayfabMatchmakingRoom(GameMode gamemode, string matchId) {
        GameMatch roomCreatedByTeamMate = FindMatchRoomByMatchId(matchId);
        if (roomCreatedByTeamMate != null) {
            return roomCreatedByTeamMate;
        }
        else {
            return CreateMatchRoom(gamemode, matchId);
        }
    }

    [ServerCallback]
    private GameMatch FindMatchRoomByMatchId(string matchId) {
        foreach (GameMatch match in unStartedMatchList) {
            if (match.MatchId == matchId) {
                return match;
            }
        }

        return null;
    }

    /// <summary>
    /// Helper method. Help to increase the currentPort by 1
    /// </summary>
    [ServerCallback]
    private void UpdatePort() {
        do {
            currentPort++;
            if (currentPort > Max_Port) {
                currentPort = Min_Port;
            }
        } while (CheckPortDuplicate(currentPort));

    }

    private bool CheckPortDuplicate(ulong port) {
        for (int i = 0; i < startedMatchList.Count; i++) {
            if (port == startedMatchList[i].Port) {
                return true;
            }
        }
        return false;
    }

    private void HandleOnMatchStartingProcess(GameMatch match) {
        unStartedMatchList.Remove(match);
        startedMatchList.Add(match);
    }
}
