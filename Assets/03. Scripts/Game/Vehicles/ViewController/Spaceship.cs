using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Utilities;
using Mirror;
using Unity.Mathematics;
using UnityEngine;


public abstract class Spaceship : Vehicle {
    private float upAxis = 0;
    private float rotateAxis = 0;

    [SerializeField] 
    private Transform avatar;
    
    public abstract void Attack();
    public abstract string GenerateName();


    [Command]
    private void CmdAccelerateControl(bool accelerate) {
        vehicleModel.Accelerate(accelerate);
    }


    [Command]
    private void CmdRotateControl(Vector3 direction) {
        rotateAxis = direction.y;
        (vehicleModel as SpaceshipModel).AddAngularVelocity(direction);
    }


    [Command]
    private void CmdUpDown(float axis) {
        upAxis = axis;
        (vehicleModel as SpaceshipModel).AddUpDownSpeed(axis);
    }



    [ServerCallback]
    public override void ServerMoveControl() {
        SpaceshipModel model = vehicleModel as SpaceshipModel;

        rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.Euler(model.CurrentRotateAngle),
            model.AngularDamp * Time.deltaTime));

        
        //var moveDirection = transform.up * model.UpSpeed + transform.forward * model.ForwardSpeed;
        //rigidbody.MovePosition(transform.position + moveDirection*Time.deltaTime);

        PhysicsUtility.RigidbodyMoveForward(rigidbody,model.ForwardSpeed);
        PhysicsUtility.RigidbodyMoveUpward(rigidbody,model.UpSpeed);
       
        ServerRollPitch(model);

    }

    [ServerCallback]
    private void OnCollisionEnter(Collision other) {
        SpaceshipModel model = vehicleModel as SpaceshipModel;
        model.UpSpeed *= -1;
        model.ForwardSpeed *= -1;
        model.AngularVelocity *= -1;
    }

    [ServerCallback]
    private void ServerRollPitch(SpaceshipModel model) {
        avatar.localRotation = Quaternion.Lerp(avatar.localRotation, Quaternion.Euler(
                model.UpAngle * -upAxis, 0,  model.RotateAngle * -rotateAxis),
            model.AngleInterpolate * Time.deltaTime);
    }


    [Client]
    public override void ClientMoveControl() {
        if (hasAuthority) {
            ClientAccelerateControl();
            ClientUpDownControl();
            ClientRotateControl();
        }
    }

  

    [Client]
    private void ClientAccelerateControl() {
        int axis = GetAxis(GameControlSettings.KEY_ACCELERATE,GameControlSettings.KEY_DECELERATE);
        CmdAccelerateControl(axis==1);
        
    }

    [Client]
    private void ClientUpDownControl() {
        int axis = GetAxis(GameControlSettings.KEY_UP,GameControlSettings.KEY_DOWN);
        CmdUpDown(axis);
    }


    [Client]
    private void ClientRotateControl() {
        

        float yawRotateAxis = GetAxis(GameControlSettings.KEY_RIGHT, GameControlSettings.KEY_LEFT);
        
        float rollAxis = 0;
        if (Input.mousePosition.x >= Screen.width)
        {
            rollAxis = -6;
        }
        else if (Input.mousePosition.x <= 0)
        {
            rollAxis = 6;
        }

        float pitchAxis = 0;
        if (Input.mousePosition.y >= Screen.height)
        {
            pitchAxis = -6;
        }
        else if (Input.mousePosition.y <= 0)
        {
            pitchAxis = 6;
        }

        Vector3 direction = new Vector3(pitchAxis, yawRotateAxis, rollAxis);
        CmdRotateControl(direction);
    }
    

}
