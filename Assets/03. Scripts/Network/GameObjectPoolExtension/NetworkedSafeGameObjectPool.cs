using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework;
using MikroFramework.Pool;
using Mirror;
using UnityEngine;

public class NetworkedSafeGameObjectPool:SafeGameObjectPool {
    
    private void Start() {
        destroyedObjectInQueue = new Queue<GameObject>();
        NetworkClient.RegisterPrefab(pooledPrefab, SpawnHandler, UnSpawnHandler);
        Init(pooledPrefab,initCount,maxCount);
    }

    protected override void Init(GameObject pooledPrefab) {
        throw new NotImplementedException();
    }

    private GameObject SpawnHandler(SpawnMessage msg) {
        GameObject allocated = Allocate();
        allocated.transform.position = msg.position;
        allocated.transform.rotation = msg.rotation;
        return allocated;
    }

    private void UnSpawnHandler(GameObject spawned) {
        Recycle(spawned);
    }

}
