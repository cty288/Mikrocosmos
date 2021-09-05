using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using Mirror;
using UnityEngine;

public class VehicleModel {

    private int Id;
    

    private string Name;


    private float speed;
    public float Speed=> speed;


    private float Acceleration;


    private float MaxAcceleration=2f;
    

    private float Hp;

    public void AddAcceleration(float addAmount) {
        Acceleration += addAmount;
        if (Acceleration > MaxAcceleration)
        {
            Acceleration = MaxAcceleration;
        }
    }

    /*
    private void Update() {
        speed += Acceleration;
    }*/
}
