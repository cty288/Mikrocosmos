using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System;
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
        EventCenter.AddListener<GameMatch>(EventType.GAME_OnMatchExited, HandleOnMatchExited);
    }

    private void OnDestroy() {
        EventCenter.RemoveListener<GameMatch>(EventType.MENU_OnServerMatchStartingProcess, HandleOnMatchStartingProcess);
        EventCenter.RemoveListener<GameMatch>(EventType.GAME_OnMatchExited, HandleOnMatchExited);
    }
    /// <summary>
    /// Return an available game match
    /// </summary>
    /// <param name="gamemode"></param>
    /// <returns></returns>
    [ServerCallback]
    public GameMatch FindAvailableMatch(GameMode gamemode) {
        unStartedMatchList = ShuffleMatch(unStartedMatchList);

        foreach (GameMatch match in unStartedMatchList) {
            if (match.Gamemode.getGameMode() == gamemode.getGameMode()) {
                if (match.GetCurrentPlayerNumber() < match.GetRequiredPlayerNumber()) {
                    return match;
                }
            }
        }
        return null;
    }

    [ServerCallback]
    public List<GameMatch> ShuffleMatch(List<GameMatch> original)
    {
        System.Random randomNum = new System.Random();
        int index = 0;
        GameMatch temp;

        for (int i = 0; i < original.Count; i++)
        {
            index = randomNum.Next(0, original.Count - 1);
            if (index != i)
            {
                temp = original[i];
                original[i] = original[index];
                original[index] = temp;
            }
        }
        return original;
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
            if (match != null && match.MatchId == matchId) {
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
        foreach (GameMatch match in startedMatchList)
        {
            if (match != null && port == match.Port)
            {
                return true;
            }
        }
        return false;
    }

    private void HandleOnMatchStartingProcess(GameMatch match) {
        unStartedMatchList.Remove(match);
        startedMatchList.Add(match);
    }

    private void HandleOnMatchExited(GameMatch match)
    {
        if (startedMatchList.Contains(match))
        {
            startedMatchList.Remove(match);
            Destroy(match.gameObject);
            Debug.Log($"Match {match.MatchId} has exited. It is destroyed from the MatchManager");
        }
        else
        {
            Debug.Log($"Match {match.MatchId} has exited, but we couldn't locate it in MatchManager," +
                      $"while its gameobject has been destroyed");
            Destroy(match.gameObject);
        }
    }
}

