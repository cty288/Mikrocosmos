using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vehicle : MonoBehaviour {
    protected int id;
    protected string name;
    protected float speed;
    protected float acceleration;
    protected float hp;

    public abstract void Move();
    public abstract void Init();

    void Start() {
        Init();
    }
}
