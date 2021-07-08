using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MirrorServerUtilities : MonoBehaviour
{
    /// <summary>
    /// Add a server only object (with specified component) to the server
    /// </summary>
    /// <typeparam name="T">Custom component added to the object</typeparam>
    /// <param name="addedName">Name of the object</param>
    public static GameObject SpawnServerOnlyObject<T>(string addedName) where T:Component
    {
        if (NetworkServer.active) {
            
            GameObject addedObj = new GameObject(addedName);
            addedObj.AddComponent<NetworkIdentity>().serverOnly=true;
            addedObj.AddComponent<T>();
            Debug.Log($"[MirrorServerUtilities] {addedName} added to the server!");
            return addedObj;
        }

        Debug.Log("[MirrorServerUtilities] Failed to add " + addedName);
        return null;
    }
}
