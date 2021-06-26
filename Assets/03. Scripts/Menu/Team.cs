using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Plugins;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class Team {
    private List<Faction> factions;
    private int totalPlayerNumber;
    private int currentPlayerNumber;
    private List<PlayerTeamInfo> playerTeamInfos;
    
    public Team(int teamNumber, int totalPlayerNumber) {
        this.totalPlayerNumber = totalPlayerNumber;
        this.currentPlayerNumber = 0;
        playerTeamInfos = new List<PlayerTeamInfo>();

        factions = new List<Faction>();
        for (int i = 0; i < teamNumber; i++)
        {
            factions.Add(new Faction());
        }

        int playerPerTeam = totalPlayerNumber / teamNumber; //e.g. total=19, teamNum=4, ppt=4
        int remainder = this.totalPlayerNumber % teamNumber; //e.g. remainder=3

        for (int i = 0; i < factions.Count; i++) {
            factions[i].MaxSize = playerPerTeam; 
        }

        for (int i = 0; i < remainder; i++) {
            factions[i].MaxSize++;
        }

    }

    /// <summary>
    /// return the number of faction
    /// </summary>
    /// <returns></returns>
    public int GetFactionNumber() {
        return factions.Count;
    }


    /// <summary>
    /// Add player to a random and available team, return if add success
    /// </summary>
    /// <param name="player"></param>
    /// <param name="teamNum">The id of the team (0 is team1, 1 is team2,etc.)</param>
    /// <returns></returns>
    public bool AddPlayerToTeam(MasterServerPlayer player, PlayerTeamInfo playerTeamInfo) {
        if (currentPlayerNumber >= totalPlayerNumber) {
            playerTeamInfo.teamId = -1;
            return false;
        }

        if (IsSamePlayerExist(player)) {
            playerTeamInfo.teamId = -1;
            return false;
        }

        int teamId = Random.Range(0, factions.Count);
        Faction faction = factions[teamId];

        while (faction.isFull()) {
            teamId = Random.Range(0, factions.Count);
            faction = factions[teamId];
        }

        faction.members.Add(player);
        
        currentPlayerNumber++;
        playerTeamInfo.teamId = teamId;
        playerTeamInfos.Add(playerTeamInfo);
        return true;
    }

    private bool AddPlayerToTeam(MasterServerPlayer player, int faction, PlayerTeamInfo playerTeamInfo) {
        if (factions[faction].isFull()) {
            return false;
        }

        if (IsSamePlayerExist(player)) {
            return false;
        }
        factions[faction].members.Add(player);
        playerTeamInfo.teamId = faction;
        playerTeamInfos.Add(playerTeamInfo);
        currentPlayerNumber++;
        return true;
    }



    private bool IsSamePlayerExist(MasterServerPlayer player) {
        foreach (Faction faction in factions) {
            foreach (MasterServerPlayer existingPlayer in faction.members) {
                if (existingPlayer.DisplayName == player.DisplayName) {
                    return true;
                }
            }
        }

        return false;
    }

    public void RemovePlayerFromTeam(MasterServerPlayer player) {
        for (int i = 0; i < factions.Count; i++) {
            for (int j = 0; j < factions[i].members.Count; j++) {
                string displayName = factions[i].members[j].DisplayName;
                if (displayName == player.DisplayName) {
                    
                    factions[i].members.RemoveAt(j);
                    currentPlayerNumber--;
                    RemovePlayerFromPlayerTeamInfos(displayName);
                    return;
                }
            }
        }

        

    }

    private void RemovePlayerFromPlayerTeamInfos(string displayName) {
        for (int i = 0; i < playerTeamInfos.Count; i++)
        {
            if (displayName == playerTeamInfos[i].DisplayName)
            {
                Debug.Log($"Removed {displayName} from server team");
                playerTeamInfos.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// Is the team full?
    /// </summary>
    /// <returns></returns>
    public bool IsFull() {
        return currentPlayerNumber >= totalPlayerNumber;
    }

    /// <summary>
    /// return a list of all existing name in the match
    /// </summary>
    /// <returns></returns>
    public List<List<string>> GetExistingFactionNameList() {
        List<List<string>> result = new List<List<string>>();
        for (int i = 0; i < factions.Count; i++) {
            for (int j = 0; j < factions[i].members.Count; j++) {
                result[i][j] = factions[i].members[j].DisplayName;
            }
        }

        return result;
    }

    public string[] GetNameList(int faction) {
        List<MasterServerPlayer> members = factions[faction].members;
        string[] names = new string[members.Count];
        for (int i = 0; i < names.Length; i++) {
            names[i] = members[i].DisplayName;
        }

        return names;
    }

    /// <summary>
    /// Return an array of PlayTeamInfo, which includes the information of which player is in which team
    /// </summary>
    /// <returns></returns>
    public List<PlayerTeamInfo> GetExistingPlayerTeamInfos() {
        return playerTeamInfos;
    }
}

public class PlayerTeamInfo {
    public string DisplayName;
    public int teamId;
    public string matchId;

    public PlayerTeamInfo(string displayName, int teamId,string matchId) {
        this.DisplayName = displayName;
        this.teamId = teamId;
        this.matchId = matchId;
    }

    public PlayerTeamInfo()
    {
        this.DisplayName = "";
        this.teamId = -1;
        this.matchId = "";
    }
    public override bool Equals(object other)
    {
        if (other == null) { return false; }
        if (GetType() != other.GetType()) { return false; }

        PlayerTeamInfo otherInfo = (PlayerTeamInfo)other;

        if (otherInfo.DisplayName == DisplayName && otherInfo.teamId == teamId && otherInfo.matchId == matchId) {
            return true;
        }
        return false;
    }
}

public class Faction {
    private int maxSize = 0;
    public int MaxSize {
        get => maxSize;
        set => maxSize = value;
    }

    public List<MasterServerPlayer> members;

    public Faction() {
        members = new List<MasterServerPlayer>();
    }

    /// <summary>
    /// get the current size of the faction.
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSize() {
        return members.Count;
    }

    public bool isFull() {
        return members.Count >= maxSize;
    }
}
