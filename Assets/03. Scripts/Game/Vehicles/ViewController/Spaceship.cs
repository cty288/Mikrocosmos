using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using Mirror;
using UnityEngine;


public abstract class Spaceship : Vehicle {
    
    protected override void Awake() {
        base.Awake();
       // vehicleModel = GetComponent<SpaceshipModel>();
       // Debug.Log("awake!!!");
    }

    public abstract void Attack();

    public abstract string GenerateName();

    [Command]
    private void CmdAccelerateControl(float amount) {
        
        this.SendCommand(SpaceshipAccelerateCommand.Allocate(vehicleModel as SpaceshipModel, amount));
    }


    [Client]
    public override void ClientMoveControl() {
        if (hasAuthority) {
            if (Input.GetKey(GameControlSettings.KEY_ACCELERATE)) {
                CmdAccelerateControl(0.1f);
            }

            if (Input.GetKey(GameControlSettings.KEY_DECELERATE)) {
                CmdAccelerateControl(-0.1f);
            }
        }
    }
}
