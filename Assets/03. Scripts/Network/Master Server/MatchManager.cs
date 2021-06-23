using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MatchManager : NetworkBehaviour {
    private List<GameMatch> unStartedMatchList;
    private List<GameMatch> startedMatchList;
    private ulong currentPort = 7778;
    private const ulong Max_Port = 60000;
    private const ulong Min_Port = 7778;
    
    [ServerCallback]
    void Awake() {
        unStartedMatchList = new List<GameMatch>();
        startedMatchList = new List<GameMatch>();
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
    public GameMatch CreateMatchRoom(GameMode gamemode, string matchId) {
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
    /// Helper method. Help to increase the currentPort by 1
    /// </summary>
    [ServerCallback]
    private void UpdatePort() {
        currentPort++;
        if (currentPort > Max_Port) {
            currentPort = Min_Port;
        }
    }
}
