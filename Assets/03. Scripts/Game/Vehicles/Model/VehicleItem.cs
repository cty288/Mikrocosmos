using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using Mirror;
using UnityEngine;

public class VehicleItem {

    public int Id;

    public string Name;

    public float ForwardSpeed;
    public float MaxForwardSpeed;
    public float ForwardAcceleration;
    public float ForwardDamp;


    public int Hp;

    public int MaxHp;


    public void Accelerate(bool speedUp) {
        int direction = speedUp ? 1 : -1;

        if (direction > float.Epsilon || direction < -float.Epsilon) {
            ForwardSpeed = Mathf.Lerp(ForwardSpeed, ForwardSpeed + ForwardAcceleration * direction, Time.deltaTime);
        }
        else {
            ForwardSpeed = Mathf.Lerp(ForwardSpeed, 0, ForwardDamp * Time.deltaTime);
        }
      
        ForwardSpeed = Mathf.Clamp(ForwardSpeed, 0, MaxForwardSpeed);

    }
}
