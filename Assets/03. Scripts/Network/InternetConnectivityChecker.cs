using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetConnectivityChecker : MonoBehaviour {
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
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastConnectivity != hasConnectivity) {
            lastConnectivity = hasConnectivity;
            if (hasConnectivity) {
                EventCenter.Broadcast(EventType.INTERNET_OnInternetConnectionRecover);
            }
            else {
                EventCenter.Broadcast(EventType.INTERNET_OnInternetLostConnection);
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
}
