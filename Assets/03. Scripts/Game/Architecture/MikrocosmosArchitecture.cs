using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class MikrocosmosArchitecture : NetworkedArchitecture<MikrocosmosArchitecture>
{

#if UNITY_EDITOR || UNITY_SERVER
    protected override void SeverInit() {
        RegisterSystem<IVehicleSystem>(VehicleSystem.Singleton);
    }
#endif

    protected override void ClientInit() {
        RegisterSystem<IVehicleSystem>(VehicleSystem.Singleton);
    }

   
}
