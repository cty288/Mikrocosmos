using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientObject : MonoBehaviour {
    void Awake() {
#if UNITY_SERVER
        this.gameObject.SetActive(false);  
#endif
    }
}
