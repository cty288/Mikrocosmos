using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using Mirror;
using UnityEngine;

public abstract class Vehicle : AbstractNetworkedController<MikrocosmosArchitecture> {
    //[SyncVar]
    protected VehicleModel vehicleModel;


    private Rigidbody rigidbody;

    protected virtual void Awake() {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if (isClient && hasAuthority) {
            ClientMoveControl();
        }

        if (isServer) {
            if (vehicleModel != null) {
                //move
              //  Debug.Log(rigidbody==null);
                rigidbody.velocity = new Vector3(0, 0, vehicleModel.Speed);
            }
            
        }
    }

    public override void OnStartClient() {
        ClientInit();
    }

    public override void OnStartServer() {
        ServerInit();
    }


    public abstract void ClientMoveControl();


    protected abstract void ClientInit();

    protected abstract void ServerInit();
}
