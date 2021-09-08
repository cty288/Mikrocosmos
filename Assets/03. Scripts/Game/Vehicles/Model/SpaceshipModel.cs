using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

public class SpaceshipModel : VehicleModel
{
    public float attack;

    public Vector3 AngularVelocity;

    public float MaxAngularSpeed;
    public float AngularAcceleration;
    public float AngularDamp;

    public float UpSpeed;
    public float MaxUpSpeed;
    public float UpAcceleration;
    public float UpDamp;

    public Vector3 CurrentRotateAngle;

    public float UpAngle;
    public float RotateAngle;
    public float AngleInterpolate;



    public void AddAngularVelocity(Vector3 direction) {
        if (direction.sqrMagnitude>float.Epsilon) {
            AngularVelocity +=  AngularAcceleration * direction * Time.deltaTime;
        }
        else {
            AngularVelocity = Vector3.Lerp(AngularVelocity, Vector3.zero, AngularDamp * Time.deltaTime);
        }
        
        AngularVelocity = Vector3.ClampMagnitude(AngularVelocity, MaxAngularSpeed);

        CurrentRotateAngle += AngularVelocity *Time.deltaTime;

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
