using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkConnector : MonoBehaviour {
    public static NetworkConnector _singleton;

    private string ipAddress = "127.0.0.1";
    private ushort port = 0;

    private float mirrorServerConnectionTimeout=10f;
    private bool startDetectServerConnect = false;

    
    
    void Awake() {
        _singleton = this;
    }

    void Update() {
        if (startDetectServerConnect) {
            if (CheckConnected() && NetworkManager.singleton.networkAddress==ipAddress
            && NetworkManager.singleton.GetComponent<TelepathyTransport>().port == port) {
                StopAllCoroutines();
                startDetectServerConnect = false;
            }
        }
    }

    /// <summary>
    /// Connect to a Mirror server with an address and port. MIRROR_OnMirrorConnectSuccess and MIRROR_OnMirrorConnectTimeout
    /// are thrown when successfully connect to the target server or time out
    /// </summary>
    /// <param name="address"></param>
    /// <param name="port">Master Server: 7777; other servers: 7778+. Check ServerInfo class for more information</param>
    /// <param name="minimumWaitTime">The minimum amount of time (in seconds) the user needs to wait before connecting. The default value is 1.5</param>
    /// <returns></returns>
    public void ConnectToServer(string address, ushort port, float minimumWaitTime=1.5f) {
        StartCoroutine(NetworkConnector.GetOrCreate(gameObject).Connect(address, port,minimumWaitTime));
    }



    private IEnumerator Connect(string address, ushort port,float minimumWaitTime=1.5f) {
        yield return new WaitForSeconds(minimumWaitTime);

       NetworkManager.singleton.StopClient();
       yield return new WaitForSeconds(0.5f);
        
        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.GetComponent<TelepathyTransport>().port = port;
        this.ipAddress = address;
        this.port = port;
        NetworkManager.singleton.StartClient();
        startDetectServerConnect = true;

        yield return new WaitForSeconds(mirrorServerConnectionTimeout);
        startDetectServerConnect = false;
        if (!NetworkClient.isConnected) { 
            EventCenter.Broadcast(EventType.MIRROR_OnMirrorConnectTimeout);
        }
        
    }


    /// <summary>
    /// Handle a weird Unity bug when calling coroutine on a singleton
    /// When call a coroutine using the singleton of this class, always use this method
    /// Example: StartCoroutine(NetworkConnector.GetOrCreate(gameObject).xxx)
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static NetworkConnector GetOrCreate(GameObject gameObject)
    {
        return gameObject.GetComponent<NetworkConnector>();
    }

    private bool CheckConnected() {
        if (NetworkClient.isConnected) {
            return true;
        }

        return false;
    }
}
