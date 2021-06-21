using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Test : NetworkBehaviour {
    public static Test _instance;

    public override void OnStartServer() {
        _instance = this;
    }

    [Server]
    public void CallClientTest() {
        ((MasterServerNetworkManager)NetworkManager.singleton).players[0].RpcTest();
    }
}
