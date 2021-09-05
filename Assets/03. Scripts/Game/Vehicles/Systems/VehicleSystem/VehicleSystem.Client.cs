using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Singletons;
using Mirror;
using UnityEngine;

public partial class VehicleSystem : AbstractNetworkedSystem, IVehicleSystem, ISingleton
{
    [ClientCallback]
    protected override void OnClientInit() {
        
    }

    private void Update() {
        if (isClient) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                CmdCreateNewVehicle(VehicleType.Spaceship,(int) SpaceshipType.SeriesC);
            }
        }
    }
}
