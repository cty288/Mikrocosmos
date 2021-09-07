using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Factory;
using MikroFramework.Pool;
using MikroFramework.ResKit;
using Mirror;
using UnityEngine;

public class VehicleFactory{
    /// <summary>
    /// Create and spawn a vehicle on the server
    /// </summary>
    /// <param name="vehicleType"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static GameObject CreateVehicle(VehicleType vehicleType, int id) {
        ResLoader resLoader = new ResLoader();
        GameObject spawnedGameObject = null;

        switch (vehicleType) {
            case VehicleType.Spaceship:
                SpaceshipConfigModel configModel=SpaceshipConfigModel.Singleton;
                IEnumerator<SpaceshipConfig> spaceshipConfigEnumerator = configModel.IdIndex.Get(id).GetEnumerator();
                spaceshipConfigEnumerator.MoveNext();
                SpaceshipConfig currentSpaceshipConfig = spaceshipConfigEnumerator.Current;
                Debug.Log(currentSpaceshipConfig.Name);

                if (id == (int) SpaceshipType.SeriesC) {
                    spawnedGameObject = GameEntrance.Singleton.POOL_SeriesC.Allocate();
                    SpaceshipModel model = CreateSpaceshipModel(currentSpaceshipConfig);
                    spawnedGameObject.GetComponent<Spaceship>().SetVehicleModel(model);
                    return spawnedGameObject;
                }
                break;
            case VehicleType.Robot:
                break;
        }
        return null;
    }

    private static SpaceshipModel CreateSpaceshipModel(SpaceshipConfig currentSpaceshipConfig) {
        SpaceshipModel model = new SpaceshipModel()
        {
            ForwardAcceleration = currentSpaceshipConfig.ForwardAcceleration,
            MaxForwardSpeed = currentSpaceshipConfig.MaxForwardSpeed,
            ForwardDamp = currentSpaceshipConfig.ForwardDamp,

            MaxAngularSpeed = currentSpaceshipConfig.MaxAngularSpeed,
            AngularAcceleration = currentSpaceshipConfig.AngularAcceleration,
            AngularDamp = currentSpaceshipConfig.AngularDamp,

            MaxUpSpeed = currentSpaceshipConfig.MaxUpSpeed,
            UpAcceleration = currentSpaceshipConfig.UpAcceleration,
            UpDamp = currentSpaceshipConfig.UpDamp,
           
            attack = currentSpaceshipConfig.Attack,
            Id = currentSpaceshipConfig.Id,
            MaxHp = currentSpaceshipConfig.MaxHp,
            Name = currentSpaceshipConfig.Name,

            UpAngle = currentSpaceshipConfig.UpAngle,
            RotateAngle = currentSpaceshipConfig.RotateAngle,
            AngleInterpolate = currentSpaceshipConfig.AngleInterpolate
        };

        return model;
    }
}
