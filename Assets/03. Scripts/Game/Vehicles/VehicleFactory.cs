using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Factory;
using MikroFramework.Pool;
using MikroFramework.ResKit;
using Mirror;
using UnityEngine;

public class VehicleFactory{
    public static GameObject CreateVehicle(VehicleType vehicleType, int id) {
        ResLoader resLoader = new ResLoader();
        GameObject spawnedGameObject = null;

        switch (vehicleType) {
            case VehicleType.Spaceship:
                if (id == (int) SpaceshipType.SeriesC) {
                    spawnedGameObject =GameObject.Instantiate(resLoader.LoadSync<GameObject>("Spaceship_TypeC"));

                    NetworkServer.Spawn(spawnedGameObject);
                    return spawnedGameObject;
                }
                break;
            case VehicleType.Robot:
                break;
        }
        return null;
    }
}
