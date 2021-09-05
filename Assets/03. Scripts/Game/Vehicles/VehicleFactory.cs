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
                if (id == (int) SpaceshipType.SeriesC) {
                    spawnedGameObject = GameEntrance.Singleton.POOL_SeriesC.Allocate();
                    return spawnedGameObject;
                }
                break;
            case VehicleType.Robot:
                break;
        }
        return null;
    }
}
