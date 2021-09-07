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

    [ClientCallback]
    private void Update() {
        if (isClient) {
            if (Input.GetKeyDown(KeyCode.RightShift)) {
                CmdCreateNewVehicle(VehicleType.Spaceship,(int) SpaceshipType.SeriesC, connectionToClient as NetworkConnectionToClient);
                Debug.Log("233");
            }
        }
    }

    [TargetRpc]
    private void TargetSpaceshipCreated(NetworkConnection conn, GameObject spaceship) {
        SpaceshipCamera camera = Camera.main.GetComponent<SpaceshipCamera>();
        camera.Target = spaceship;
    }

}
