using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using UnityEngine;
using EventType = MikroFramework.Event.EventType;

public class InternetConnectivityChecker : MikroBehavior {
    public static InternetConnectivityChecker _instance;

    [SerializeField] [Tooltip("If this is empty, the checker will only check whether the network is reachable via WiFi/cable/Carrier; otherwise, the checker will only" +
                              "check whether the network is reachable to this specific website")]
    private string specificCheckingWebsite="";
    private bool lastConnectivity = true;
    private bool hasConnectivity = true;

    /// <summary>
    /// Return true if the network is reachable (or reachable to the specific website in InternetConnectivityChecker instance)
    /// </summary>
    public bool HasConnectivity => hasConnectivity;



    [SerializeField] 
    private float checkingTimeInterval = 1f;

    private float timer = 0;

    void Awake() {
        if (InternetConnectivityChecker._instance != null) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
    }

    // Update is called once per frame
        void Update()
    {
        if (lastConnectivity != hasConnectivity) {
            lastConnectivity = hasConnectivity;
            if (hasConnectivity) {
                Broadcast(EventType.INTERNET_OnInternetConnectionRecover,null);
            }
            else {
                Broadcast(EventType.INTERNET_OnInternetLostConnection,null);
            }
        }

        if (specificCheckingWebsite!="") {
            timer += Time.deltaTime;
            if (timer >= checkingTimeInterval) {
                timer = 0;
                StartCoroutine(checkInternetConnection());
            }
            
        }
        else {
            if (Application.internetReachability == NetworkReachability.NotReachable) {
                hasConnectivity = false;
            }
            else {
                hasConnectivity = true;
            }
        }
    }

    IEnumerator checkInternetConnection() {
        WWW www = new WWW(specificCheckingWebsite);
        yield return www;
        if (www.error != null) {
            hasConnectivity = false;
        }
        else {
            hasConnectivity = true;
        }
    }

    protected override void OnBeforeDestroy() {
        
    }
}
