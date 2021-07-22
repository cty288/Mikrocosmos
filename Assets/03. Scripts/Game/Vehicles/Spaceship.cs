using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spaceship : Vehicle {
    protected float attack;
    protected float angularSpeed;

    public abstract void Attack();

    public abstract string GenerateName();

    public override void Init() {
        name = GenerateName();
    }
}
