using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class MasterServerPlayer : NetworkBehaviour {
    [SyncVar] 
    private long timeIntervalPerCommand = 300;

    [SyncVar] private long lastCommandTick = 0;

    #region Server
    [Command]
    private void CmdUpdateLastCommandTick() {
        lastCommandTick = DateTime.Now.Ticks;
    }

    #endregion

    #region Client

    /// <summary>
    /// Call this to run Cmd functions. This function ensures the player is not requesting the server too frequently
    /// </summary>
    /// <param name="serverCommand"></param>
    [Client]
    private void RunServerCommand(Action serverCommand) {
        if (hasAuthority) {
            TimeSpan startSpan = new TimeSpan(lastCommandTick);
            TimeSpan nowSpan = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();

            if (subTimer.TotalMilliseconds > timeIntervalPerCommand)
            {
                serverCommand.Invoke();
                CmdUpdateLastCommandTick();
            }
            else
            {
                EventCenter.Broadcast<string, UnityAction, string, object[]>(EventType.MENU_Error,
                    "MENU_REQUEST_EXCEED", () => { }, "GAME_ACTION_CLOSE", null);
            }
        }
    }

    public override void OnStartAuthority() {
        base.OnStartAuthority();
        EventCenter.Broadcast(EventType.MENU_AuthorityOnConnected);
    }

    [Client]
    public void RequestMatch(Mode gamemode) {
        if (hasAuthority) {
            print("Requesting: "+gamemode.ToString());
        }
    }
    #endregion

}
