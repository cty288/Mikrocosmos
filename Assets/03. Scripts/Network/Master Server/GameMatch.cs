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
    GameAlreadyStart,
    MatchSpawnFailed
}

public class GameMatch : NetworkBehaviour {
    private List<MasterServerPlayer> playersInMatch;
    public List<MasterServerPlayer> PlayersInMatch => playersInMatch;

    private Team team;

    private string matchId;
    public string MatchId => matchId;

    private ushort port;
    public ushort Port => port;

    private string ip;
    public string Ip => ip;
    
    private GameMode gamemode;
    public GameMode Gamemode => gamemode;

    private Process gameProcess;

    private MatchState matchState = MatchState.WaitingForPlayers;

    public Action<MasterServerPlayer, PlayerTeamInfo> onNewPlayerJoins;
    public Action<PlayerTeamInfo[]> teamInfoUpdate;
    //public Action<int> countdownUpdate;
    public Action<float> countdownUpdate;
    public Action<MatchState> onMatchStateChange;

   
    private float countDown = 10f;


    [ServerCallback]
    void Update()
    {
        UpdateCountDown();
    }

    private void UpdateCountDown()
    {
        if (matchState==MatchState.CountDownForMatch)
        {
            countDown -= Time.deltaTime;
            if (countDown <= 0) {
                if (team.CurrentPlayerNumber >= team.TotalPlayerNumber) {
                    UpdateServerMatchState(MatchState.StartingGameProcess);
                }
                else {
                    DetectMatchRoomFull();
                }
                
                countDown = 10;
            }
        }
        else
        {
            countDown = 10;
        }
    }


    void Awake() {
        playersInMatch = new List<MasterServerPlayer>();
        port = 0;
        ip = ServerInfo.ServerIp;
    }



    [ServerCallback]
    public void SetGamemode(GameMode gamemode,string matchId, ushort port) {
        this.gamemode = gamemode;
        this.matchId = matchId;
        this.port = port;
        team = new Team(gamemode.GetTeamNumber(), gamemode.GetRequiredPlayerNumber());
    }

    

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
            Debug.Log($"Added {player.TeamInfo.DisplayName} to match {matchId}");
        }
        else {
            Debug.Log($"The room is full, or {player.TeamInfo.DisplayName} already exists in match {matchId}!");
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

        countdownUpdate?.Invoke(countDown);
        //update again after a few secs
        StartCoroutine(UpdatePlayerTeamInfos());
        //broadcast new player join
        StartCoroutine(UpdatePlayerCountDownAgain());
        DetectMatchRoomFull();
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
        print($"{player.TeamInfo.DisplayName} exited match room {matchId}");
        playersInMatch.Remove(player);
        RemoveListener(player);
        DetectMatchRoomFull();
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


    private IEnumerator UpdatePlayerCountDownAgain() {
        yield return new WaitForSeconds(2.5f);
        countdownUpdate?.Invoke(countDown);
    }

    public MatchError LeaveMatch(MasterServerPlayer player) {
        if (matchState == MatchState.StartingGameProcess || matchState == MatchState.GameAlreadyStart) {
            return MatchError.MatchAlreadyStart;
        }

        if (team.RemovePlayerFromTeam(player)) {
            teamInfoUpdate?.Invoke(GetExistingPlayerTeamInfos());
            print($"{player.TeamInfo.username} exited match room {matchId}");
            
            RemovePlayerFromPlayerList(player);
            RemoveListener(player);

            DetectMatchRoomFull();

            return MatchError.NoError;
        }
        else {
            DetectMatchRoomFull();
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
                if (playersInMatch[i].TeamInfo.username == player.TeamInfo.username)
                {
                    playersInMatch.RemoveAt(i);
                }
            }
        }
    }

    [ServerCallback]
    private void DetectMatchRoomFull() {
        if (team.CurrentPlayerNumber >= team.TotalPlayerNumber) {
            if (matchState == MatchState.WaitingForPlayers) {
                UpdateServerMatchState(MatchState.CountDownForMatch);
            }
        }
        else { //less than
            if (matchState == MatchState.CountDownForMatch) {
               UpdateServerMatchState(MatchState.WaitingForPlayers);

            }else if (matchState == MatchState.StartingGameProcess) {
                //TODO: kick all players back to lobby; stop process
                onMatchStateChange?.Invoke(matchState);
            }
        }
    }

    [ServerCallback]
    private void UpdateServerMatchState(MatchState newState) {
        if (matchState != newState) {
            matchState = newState;
            onMatchStateChange?.Invoke(newState);

            //waiting for player and countdown are processed in the Update Method
            switch (newState) {
                case MatchState.CountDownForMatch:
                    //start coroutine, update countdown to the client for each 1 s
                    StartCoroutine(UpdateCountDownToClient());
                    break;
                case MatchState.StartingGameProcess:
                    //Ready to start game - invoke event and start the process
                    ServerStartGameProcess();
                    EventCenter.Broadcast(EventType.MENU_OnServerMatchStartingProcess,this);
                    break;
            }
        }
    }

    [ServerCallback]
    private bool ServerStartGameProcess() {
        Debug.Log($"Starting game process. Port: {port}");
        gameProcess = new Process();

        string processPath = ServerInfo.GameModePaths[(int)gamemode.getGameMode()];
        gameProcess.StartInfo.FileName = System.IO.Path.Combine(processPath, ServerInfo.ProcessName);

        gameProcess.StartInfo.Arguments = ip + " " +
                                          port + " " +
                                          matchId+" "+
                                          true;
        if (gameProcess.Start()) {
            Debug.Log("Spawning: " + gameProcess.StartInfo.FileName + "; args=" + gameProcess.StartInfo.Arguments);
            return true;
        }
        else {
            //TODO: Destroy the process and the gamematch itself
            return false;
        }
    }

    
    [ServerCallback]
    private IEnumerator UpdateCountDownToClient() {
        while (matchState == MatchState.CountDownForMatch) {
            //int countdownToInt = Mathf.RoundToInt(countDown);
            //countdownUpdate?.Invoke(countdownToInt);
            countdownUpdate?.Invoke(countDown);
            yield return new WaitForSeconds(3);
        }
    }
}

////
/// 

