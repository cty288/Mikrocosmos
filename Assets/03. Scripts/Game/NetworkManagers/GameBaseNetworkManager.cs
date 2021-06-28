using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameBaseNetworkManager : NetworkManager {
    private string ip;
    private ushort port;
    private string matchId;
    private bool isServer=false;

    [SerializeField] private bool isServerBuild = false;
    
    private void ServerSetup() {
        string[] args = Environment.GetCommandLineArgs();
       // print($"ip: {args[1]}, port: {args[2]}, matchId: {args[3]}, isServer: {args[4]}");

        if (isServerBuild) {
            print($"ip: {args[1]}, port: {args[2]}, matchId: {args[3]}, isServer: {args[4]}");
            ip = args[1];
            ushort.TryParse(args[2],out port);
            matchId = args[3];
            bool.TryParse(args[4],out isServer);

           Debug.Log(port+"  "+isServer.ToString());
           

            if (!string.IsNullOrEmpty(ip) && port >= 7778 && !string.IsNullOrEmpty(matchId)) {
                NetworkManager.singleton.networkAddress = ip;
                NetworkManager.singleton.GetComponent<TelepathyTransport>().port = port;
                NetworkManager.singleton.StartServer();
                Debug.Log($"{matchId} game process successfully spawned!");
            }
            else {
                Debug.Log($"Missing arguments! {matchId} auto close on the server!");
                Application.Quit();
            }
            
        }else {
            NetworkConnector._singleton.ConnectToServer(PlayerPrefs.GetString("ip"),
                (ushort)PlayerPrefs.GetInt("port"), OnJoinGameServerFailed, 1.5f, 2f, 160);
        }
       

    }

    private void OnJoinGameServerFailed() {
        Debug.Log("Join server failed");
    }

    void Start() {
        Debug.Log("caonima");
        ServerSetup();  
    }
}
