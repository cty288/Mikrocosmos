using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MatchManager : NetworkBehaviour {
    private List<GameMatch> unStartedMatchList;
    private List<GameMatch> startedMatchList;
    private ulong currentPort = 7778;
    [ServerCallback]
    public override void OnStartServer() {
        base.OnStartServer();
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
            if (match.Gamemode == gamemode) {
                if (match.GetCurrentPlayerNumber() < match.GetRequiredPlayerNumber()) {
                    return match;
                }
            }
        }
        return null;
    }

    [ServerCallback]
    public void CreateMatchRoom() {
        //just create the room; no playfab matchmaking
    }
}
