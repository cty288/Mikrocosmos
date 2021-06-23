using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mirror;
using UnityEngine;

public class GameMatch : NetworkBehaviour {
    private List<MasterServerPlayer> playersInMatch;
    public List<MasterServerPlayer> PlayersInMatch => playersInMatch;

    private Team team;

    private string matchId;
    public string MatchId => matchId;

    private ulong port;
    private string ip;
    public string Ip => ip;
    
    private GameMode gamemode;
    public GameMode Gamemode => gamemode;

    private Process gameProcess;
    private bool isGameAlreadyStart = false;


    void Awake() {
        playersInMatch = new List<MasterServerPlayer>();
        port = 0;
        ip = ServerInfo.ServerIp;
    }

    [ServerCallback]
    public void SetGamemode(GameMode gamemode,string matchId, ulong port) {
        this.gamemode = gamemode;
        this.matchId = matchId;
        this.port = port;
        team = new Team(gamemode.GetTeamNumber(), gamemode.GetRequiredPlayerNumber());
    }

    /// <summary>
    /// Return the current number of players in the lobby
    /// </summary>
    /// <returns></returns>
    public int GetCurrentPlayerNumber() {
        return playersInMatch.Count;
        
    }

    /// <summary>
    /// Is the match currently full?
    /// </summary>
    /// <returns></returns>
    public bool IsFull()
    {
        return team.IsFull();
    }

    /// <summary>
    /// Return the required player number
    /// </summary>
    /// <returns></returns>
    public int GetRequiredPlayerNumber() {
        return gamemode.GetRequiredPlayerNumber();
    }
}


