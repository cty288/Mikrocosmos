using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServerInfo {
    /// <summary>
    /// The ip of all the server is the same.
    /// </summary>
    public static string ServerIp = "127.0.0.1";

    public static ushort MasterServerPort = 7777;

    public static string[] GameModePaths = new[] {
        @"D:\programming\Mikrocosmos 2\GameServer", //quick
        @"D:\programming\Mikrocosmos 2\GameServer", //standard
        @"D:\programming\Mikrocosmos 2\GameServer"  //marathon
    };

    public static string[] GameModeSceneName = new[] {
        "StandardMode",
        "StandardMode",
        "StandardMode"
    };
    public static string ProcessName = "Mikrocosmos 2.exe";
}
