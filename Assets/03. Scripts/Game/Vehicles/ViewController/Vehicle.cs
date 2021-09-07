using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Utilities;
using Mirror;
using UnityEngine;

public abstract class Vehicle : AbstractNetworkedController<MikrocosmosArchitecture> {
    //[SyncVar]
    protected VehicleModel vehicleModel;


    protected Rigidbody rigidbody;

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
                PhysicsUtility.RigidbodyMoveForward(rigidbody,vehicleModel.ForwardSpeed);
                
                ServerMoveControl();
            }
            
        }
    }

    [ServerCallback]
    public void SetVehicleModel(VehicleModel model) {
        this.vehicleModel = model;
    }

    public override void OnStartClient() {
        ClientInit();
    }

    public override void OnStartServer() {
        ServerInit();
    }


    public abstract void ClientMoveControl();

    public abstract void ServerMoveControl();

    protected abstract void ClientInit();

    protected abstract void ServerInit();

    protected int GetAxis(KeyCode positiveKey, KeyCode negativeKey) {
        int axis = 0;
        if (Input.GetKey(positiveKey))
        {
            axis = 1;
        }

        if (Input.GetKey(negativeKey))
        {
            axis = -1;
        }

        return axis;
    }
}
