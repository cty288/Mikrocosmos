using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SpaceshipModel : VehicleModel
{
    [SyncVar]
    protected float attack;
    [SyncVar]
    protected float angularSpeed;
}
