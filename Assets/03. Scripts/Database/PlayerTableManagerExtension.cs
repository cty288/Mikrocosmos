using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikrocosmosNewDatabase;
using MikroFramework.DatabaseKit.NHibernate;
using UnityEngine;

public static class PlayerTableManagerExtension {
    /// <summary>
    /// Search the Player database object from the database, given the display name. (Null of not found)
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public static async Task<Player> SearchByDisplayName(this NHibernateTableManager<Player> table, string displayName)
    {
        return await table.SearchByFieldNameUniqueResult("DisplayName", displayName);
    }

    /// <summary>
    /// Search the Player database object from the database, given the user id name. (Null of not found)
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public static async Task<IList<Player>> SearchByUser(this NHibernateTableManager<Player> table, User user)
    {
        return await table.SearchByFieldName("Users", user);
    }

    /// <summary>
    /// Authenticate the displayname of the user. Create a new Player for the user if not found the displayname and returns true
    /// Also returns true of the displayname belongs to the user (which means the user already have this Player)
    /// returns false if the Player of this displayname does not belong to the user
    /// </summary>
    /// <param name="user"></param>
    /// <param name="displayName"></param>
    /// <returns></returns>
    public static async Task<bool> AuthenticateDisplayName(this NHibernateTableManager<Player> table, User user, string displayName)
    {
        Debug.Log($"Authenticating username {displayName}...");
        Player searchResult = await table.SearchByDisplayName(displayName);
        if (searchResult == null)
        {
            await table.Add(new Player() { DisplayName = displayName, Users = user });
            Debug.Log($"Authenticating new player name {displayName} success! ");
            return true;
        }

        if (searchResult.Users.Id == user.Id)
        {
            Debug.Log($"Authenticating existing player name {displayName} success! ");
            return true;
        }
        Debug.Log($"Authenticating display name {displayName} failed! It doesn't belong to {user.Username}! ");
        return false;
    }
}


