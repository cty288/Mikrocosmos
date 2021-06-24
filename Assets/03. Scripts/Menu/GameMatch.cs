using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mirror;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

    [ServerCallback]
    public bool JoinPlayer(MasterServerPlayer player) {
        bool result = team.AddPlayerToTeam(player);
        if (result) {
            Debug.Log($"Added {player.DisplayName} to match {matchId}");
        }
        else {
            Debug.Log($"{player.DisplayName} already exists in match {matchId}!");
        }
        playersInMatch.Add(player);
        player.onPlayerDisconnect += OnPlayerDisconnect;
        return result;
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

    [Server]
    private void OnPlayerDisconnect(MasterServerPlayer player) {
        team.RemovePlayerFromTeam(player);
        print($"{player.DisplayName} existed match room {matchId}");
        RemoveListener(player);
    }

    private void RemoveListener(MasterServerPlayer player) {
        player.onPlayerDisconnect -= OnPlayerDisconnect;
    }
}


