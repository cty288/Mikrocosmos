using System;
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

    public Action<MasterServerPlayer,PlayerTeamInfo> onNewPlayerJoins;

    /// <summary>
    /// Join a player into this match. The player is successfully joined if the room is not full and if they are not
    /// currently in the match.
    /// Return true if successfully joined, and invoke the event of this object "onNewPlayerJoins" on the server
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    [ServerCallback]
    public bool JoinPlayer(MasterServerPlayer player, PlayerTeamInfo teamInfo) { 
        bool result = team.AddPlayerToTeam(player, teamInfo);
        if (result) {
            Debug.Log($"Added {player.DisplayName} to match {matchId}");
        }
        else {
            Debug.Log($"The room is full, or {player.DisplayName} already exists in match {matchId}!");
            return false;
        }
        playersInMatch.Add(player);
        onNewPlayerJoins?.Invoke(player,teamInfo);
        player.onPlayerDisconnect += OnPlayerDisconnect;
        //broadcast new player join
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
        print($"{player.DisplayName} exited match room {matchId}");
        RemoveListener(player);
    }

    private void RemoveListener(MasterServerPlayer player) {
        player.onPlayerDisconnect -= OnPlayerDisconnect;
    }

    /// <summary>
    /// return a list of all existing name in the match
    /// </summary>
    /// <returns></returns>
    public List<List<string>> GetExistingFactionNameList()
    {
        return team.GetExistingFactionNameList();
    }

    /// <summary>
    /// Get an array of names from a given team
    /// </summary>
    /// <param name="faction"></param>
    /// <returns></returns>
    public string[] GetNameList(int faction)
    {
        return team.GetNameList(faction);
    }

    /// <summary>
    /// Return an array of PlayTeamInfo, which includes the information of which player is in which team
    /// </summary>
    /// <returns></returns>
    private PlayerTeamInfo[] GetExistingPlayerTeamInfos()
    {
        return team.GetExistingPlayerTeamInfos().ToArray();
    }

    public Action<PlayerTeamInfo[]> teamInfoUpdate;
    
    [ServerCallback] 
    void Start() {
        StartCoroutine(UpdatePlayerTeamInfos());
    }

    private IEnumerator UpdatePlayerTeamInfos() {
        while (!isGameAlreadyStart) {
            Debug.Log("Match refreshing...");
            teamInfoUpdate?.Invoke(GetExistingPlayerTeamInfos());
            yield return new WaitForSeconds(1);
        }
    }
}


