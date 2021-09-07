using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipCamera : MonoBehaviour {
    public GameObject Target;
    public Vector3 Offset = new Vector3(0, 2.0f, -2.7f);
    public float speed = 4f;

    private void FixedUpdate() {
        if (Target) {
            Vector3 targetPos = Target.transform.position + Offset;

            transform.position = Vector3.Slerp(transform.position, targetPos, speed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(Target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }
    }
}
