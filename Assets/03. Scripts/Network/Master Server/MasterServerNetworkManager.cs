using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MasterServerNetworkManager : NetworkManager {
    public List<MasterServerPlayer> players;

    public override void OnServerConnect(NetworkConnection conn) {
        base.OnServerConnect(conn);
        players.Add(conn.identity.GetComponent<MasterServerPlayer>());
        if (Test._instance) {
            Test._instance.CallClientTest();
        }
    }
}
