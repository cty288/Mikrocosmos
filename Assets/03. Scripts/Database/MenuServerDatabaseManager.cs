using System.Collections;
using System.Collections.Generic;
using MikrocosmosNewDatabase;
using MikroFramework.DatabaseKit.NHibernate;
using UnityEngine;

public class MenuServerDatabaseManager : ServerDatabaseManager
{
    /// <summary>
    /// Update a player's matchid on the database
    /// </summary>
    /// <param name="teamInfo"></param>
    /// <param name="matchId"></param>
    public async void AddMatchIdToDatabase(PlayerTeamInfo teamInfo, string matchId)
    {
        string displayName = teamInfo.DisplayName;
        Player searchedPlayer = await NHibernateTableManager<Player>.Singleton.SearchByDisplayName(displayName);

        if (searchedPlayer != null)
        {
            searchedPlayer.JoinedMatchid = matchId;

            bool result = await NHibernateTableManager<Player>.Singleton.Update(searchedPlayer);

            if (result)
                Debug.Log($"[ServerDatabaseManager] Successfully updated {matchId} to {displayName}'s data on the database!");
        }
        else
        {
            Debug.Log($"[ServerDatabaseManager] Failed to update {matchId} to {displayName}'s data on the database!");
        }
    }
}
