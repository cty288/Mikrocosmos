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
    private ulong port;
    private string ip;
    
    private GameMode gamemode;
    public GameMode Gamemode => gamemode;

    private Process gameProcess;
    private bool isGameAlreadyStart = false;

    public override void OnStartServer() {
        base.OnStartServer();
        playersInMatch = new List<MasterServerPlayer>();
        port = 0;
        ip = ServerInfo.ServerIp;
    }

    public void SetGamemode(GameMode gamemode) {
        this.gamemode = gamemode;
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


