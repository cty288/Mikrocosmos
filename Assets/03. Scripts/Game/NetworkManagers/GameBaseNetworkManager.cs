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

    
    private void ServerSetup() {
        string[] args = Environment.GetCommandLineArgs();
        print($"ip: {args[1]}, port: {args[2]}, matchId: {args[3]}, isServer: {args[4]}");

        if (args.Length > 1) {
            ip = args[1];
            port = ushort.Parse(args[2]);
            matchId = args[3];
            isServer = bool.Parse(args[4]);

           
            if (isServer)
            {

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
            }
        }
       

    }


    void Start() {
        ServerSetup();  
    }
}
