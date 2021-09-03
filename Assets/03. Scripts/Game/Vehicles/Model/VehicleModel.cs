using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using Mirror;
using UnityEngine;

public class VehicleModel:NetworkBehaviour {
    [SyncVar]
    private int Id;
    
    [SyncVar]
    private string Name;

    [SyncVar]
    private float speed;
    public float Speed=> speed;

    [SyncVar]
    private float Acceleration;

    [SyncVar]
    private float MaxAcceleration=2f;
    
    [SyncVar]
    private float Hp;


    [ServerCallback]
    public void AddAcceleration(float addAmount) {
        Acceleration += addAmount;
        if (Acceleration > MaxAcceleration)
        {
            Acceleration = MaxAcceleration;
        }
    }

    [ServerCallback]
    private void Update() {
        speed += Acceleration;
    }
}
