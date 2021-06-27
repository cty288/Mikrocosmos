using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mirror;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum MatchState {
    WaitingForPlayers,
    CountDownForMatch,
    StartingGameProcess,
    GameAlreadyStart
}

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

    private MatchState matchState = MatchState.WaitingForPlayers;



    [ServerCallback]
    private void UpdateMatchState(MatchState newState) {
        this.matchState = newState;
        onMatchStateChange?.Invoke(newState);
    }

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

        if (matchState == MatchState.GameAlreadyStart || matchState == MatchState.StartingGameProcess) {
            Debug.Log($"The game already starts!");
            return false;
        }

        playersInMatch.Add(player);
        onNewPlayerJoins?.Invoke(player,teamInfo);
        player.onPlayerDisconnect += OnPlayerDisconnect;

        teamInfoUpdate?.Invoke(GetExistingPlayerTeamInfos());
        //update again after a few secs
        StartCoroutine(UpdatePlayerTeamInfos());
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
        teamInfoUpdate?.Invoke(GetExistingPlayerTeamInfos());
        print($"{player.DisplayName} exited match room {matchId}");
        playersInMatch.Remove(player);
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
    public Action<MatchState> onMatchStateChange;

 
    private IEnumerator UpdatePlayerTeamInfos() {
        for (int i = 0; i < 3; i++)
        {
            if (matchState!=MatchState.GameAlreadyStart) {
                yield return new WaitForSeconds(1.5f);
                teamInfoUpdate?.Invoke(GetExistingPlayerTeamInfos());
                onMatchStateChange?.Invoke(this.matchState);
            }

        }

    }

    public MatchError LeaveMatch(MasterServerPlayer player) {
        if (matchState == MatchState.StartingGameProcess || matchState == MatchState.GameAlreadyStart) {
            return MatchError.MatchAlreadyStart;
        }

        if (team.RemovePlayerFromTeam(player)) {
            teamInfoUpdate?.Invoke(GetExistingPlayerTeamInfos());
            print($"{player.DisplayName} exited match room {matchId}");
            
            RemovePlayerFromPlayerList(player);
            RemoveListener(player);
            
            return MatchError.NoError;
        }
        else {
            return MatchError.UnableToFindPlayer;
        }
    }

    [ServerCallback]
    private void RemovePlayerFromPlayerList(MasterServerPlayer player) {
        try {
            playersInMatch.Remove(player);
        }
        catch (Exception e) {
            for (int i = 0; i < playersInMatch.Count; i++) {
                if (playersInMatch[i].DisplayName == player.DisplayName)
                {
                    playersInMatch.RemoveAt(i);
                }
            }
        }


    }
}

////
/// 

