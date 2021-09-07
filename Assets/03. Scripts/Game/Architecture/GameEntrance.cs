using System.Collections;
using System.Collections.Generic;
using Antlr.Runtime.Misc;
using MikroFramework.DatabaseKit.NHibernate;
using MikroFramework.Pool;
using MikroFramework.SceneEntranceKit;
using MikroFramework.Singletons;
using UnityEngine;
using Spaceship = MikrocosmosNewDatabase.Spaceship;

public class GameEntrance : NetworkedEntranceManager {
    public NetworkedSafeGameObjectPool POOL_SeriesC;

    public static GameEntrance Singleton {
        get {
            return GameObject.FindObjectOfType<GameEntrance>();
        }
    }

    
    private async void ReadSpaceshipConfigModelFromDatabase() { 
        
        IList<MikrocosmosNewDatabase.Spaceship> spaceships = 
            await NHibernateTableManager<MikrocosmosNewDatabase.Spaceship>.Singleton.GetAll();


        foreach (MikrocosmosNewDatabase.Spaceship spaceship in spaceships) {
            SpaceshipConfigModel.Singleton.Add(new SpaceshipConfig() {
                ForwardAcceleration = spaceship.FowardAcceleration,
                MaxForwardSpeed = spaceship.MaxFowardSpeed,
                ForwardDamp = spaceship.FowardDamp,

                MaxAngularSpeed = spaceship.MaxAngularSpeed,
                AngularAcceleration = spaceship.AngularAcceleration,
                AngularDamp = spaceship.AngularDamp,

                MaxUpSpeed = spaceship.MaxUpSpeed,
                UpAcceleration = spaceship.UpAcceleration,
                UpDamp = spaceship.UpDamp,

                Attack = spaceship.Attack,
                Id = spaceship.Id,
                MaxHp = spaceship.Hp,
                Name = spaceship.Name,
                
                UpAngle = spaceship.UpAngle,
                RotateAngle = spaceship.RotateAngle,
                AngleInterpolate = spaceship.AngleInterpolate
            });
        }


        Debug.Log("Read Spaceship Config Success!");
    }

    protected override void ClientLaunchInDevelopingMode() {
       
    }

    protected override void ClientLaunchInTestingMode() {

    }

    protected override void ClientLaunchInReleasedMode() {

    }

    protected override void ServerLaunchInDevelopingMode() {
        ReadSpaceshipConfigModelFromDatabase();
    }

    protected override void ServerLaunchInTestingMode() {
        ReadSpaceshipConfigModelFromDatabase();
    }

    protected override void ServerLaunchInReleasedMode() {
        ReadSpaceshipConfigModelFromDatabase();
    }
}
