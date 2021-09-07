using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

public class SpaceshipModel : VehicleModel
{
    public float attack;

    public float AngularSpeed;
    public float MaxAngularSpeed;
    public float AngularAcceleration;
    public float AngularDamp;

    public float UpSpeed;
    public float MaxUpSpeed;
    public float UpAcceleration;
    public float UpDamp;

    public float CurrentRotateAngle;
    public float UpAngle;
    public float RotateAngle;
    public float AngleInterpolate;



    public void AddAngularSpeed(float sign) {
        if (sign > float.Epsilon || sign < -float.Epsilon) {
            AngularSpeed += AngularAcceleration * sign * Time.deltaTime;
        }
        else {
            AngularSpeed = Mathf.Lerp(AngularSpeed, 0, AngularDamp * Time.deltaTime);
        }
        
        AngularSpeed = Mathf.Clamp(AngularSpeed, -MaxAngularSpeed, MaxAngularSpeed);

        CurrentRotateAngle = CurrentRotateAngle + AngularSpeed*Time.deltaTime;

    }

    public void AddUpDownSpeed(float sign) {
        if (sign > float.Epsilon || sign < -float.Epsilon) {
            UpSpeed += UpAcceleration * Time.deltaTime * sign;
        }
        else {
            UpSpeed = Mathf.Lerp(UpSpeed, 0, UpDamp * Time.deltaTime);
        }

        UpSpeed = Mathf.Clamp(UpSpeed, -MaxUpSpeed, MaxUpSpeed);
    }
}
