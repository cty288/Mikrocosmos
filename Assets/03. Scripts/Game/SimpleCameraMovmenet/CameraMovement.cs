using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 1f;

    void Update() {
        if (Input.GetKey(KeyCode.RightArrow)) {
            Vector3 target = transform.position + Vector3.right*speed;
            transform.position = Vector3.Lerp(transform.position, target, 0.1f);
        }


        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 target = transform.position + Vector3.left * speed;
            transform.position = Vector3.Lerp(transform.position, target, 0.1f);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 target = transform.position + Vector3.back * speed;
            transform.position = Vector3.Lerp(transform.position, target, 0.1f);
        }
            
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 target = transform.position + Vector3.forward * speed;
            transform.position = Vector3.Lerp(transform.position, target, 0.1f);
        }
           
    }
}
