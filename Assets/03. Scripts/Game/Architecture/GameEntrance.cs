using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using MikroFramework.SceneEntranceKit;
using MikroFramework.Singletons;
using UnityEngine;

public class GameEntrance : EntranceManager {
    public SafeGameObjectPool POOL_SeriesC;

    public static GameEntrance Singleton {
        get {
            return GameObject.FindObjectOfType<GameEntrance>();
        }
    }

    protected override void LaunchInDevelopingMode() {
        
    }

    protected override void LaunchInTestingMode() {
        
    }

    protected override void LaunchInReleasedMode() {

    }

}
