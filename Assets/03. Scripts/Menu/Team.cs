using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Team {
    private List<Faction> factions;
    private int totalPlayerNumber;
    private int currentPlayerNumber;
    
    public Team(int teamNumber, int totalPlayerNumber) {
        this.totalPlayerNumber = totalPlayerNumber;
        this.currentPlayerNumber = 0;
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
    public bool AddPlayerToTeam(MasterServerPlayer player) {
        if (currentPlayerNumber >= totalPlayerNumber) {
            return false;
        }

        if (IsSamePlayerExist(player)) {
            return false;
        }

        Faction faction = factions[Random.Range(0, factions.Count)];

        while (faction.isFull()) {
            faction = factions[Random.Range(0, factions.Count)];
        }

        faction.members.Add(player);
        currentPlayerNumber++;
        return true;
    }

    private bool AddPlayerToTeam(MasterServerPlayer player, int faction) {
        if (factions[faction].isFull()) {
            return false;
        }

        if (IsSamePlayerExist(player)) {
            return false;
        }
        factions[faction].members.Add(player);
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
                if (factions[i].members[j].DisplayName == player.DisplayName) {
                    factions[i].members.RemoveAt(j);
                    currentPlayerNumber--;
                    return;
                }
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
