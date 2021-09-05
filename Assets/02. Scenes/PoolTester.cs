using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PoolTester : NetworkBehaviour {
    public NetworkedSafeGameObjectPool pool;

    public List<GameObject> gos;

    [ServerCallback]
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GameObject allocated = pool.Allocate();
            NetworkServer.Spawn(allocated);
            gos.Add(allocated);
            
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            pool.Recycle(gos[0]);
            NetworkServer.UnSpawn(gos[0]);
            gos.RemoveAt(0);
        }
    }
}
