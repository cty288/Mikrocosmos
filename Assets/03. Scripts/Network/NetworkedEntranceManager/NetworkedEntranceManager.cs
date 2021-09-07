using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.SceneEntranceKit;
using Mirror;
using UnityEngine;

public abstract class NetworkedEntranceManager : NetworkBehaviour {
    [SerializeField]
    protected EnvironmentMode environmentMode;
    public EnvironmentMode EnvironmentMode => environmentMode;

    private static EnvironmentMode sharedMode;
    private static bool modeSet = false;


    public override void OnStartClient() {
        switch (sharedMode)
        {
            case EnvironmentMode.Developing:
                ClientLaunchInDevelopingMode();
                break;
            case EnvironmentMode.Testing:
                ClientLaunchInTestingMode();
                break;
            case EnvironmentMode.Released:
                ClientLaunchInReleasedMode();
                break;
        }
    }

    public override void OnStartServer() {
        switch (sharedMode)
        {
            case EnvironmentMode.Developing:
                ServerLaunchInDevelopingMode();
                break;
            case EnvironmentMode.Testing:
                ServerLaunchInTestingMode();
                break;
            case EnvironmentMode.Released:
                ServerLaunchInReleasedMode();
                break;
        }
    }

    void Start()
    {
        if (!modeSet)
        {
            sharedMode = environmentMode;
            modeSet = true;
        }
        else
        {
            environmentMode = sharedMode;
        }

    }

    /// <summary>
    /// Code in Developing Mode will only run at the development phrase of the project
    /// </summary>
    protected abstract void ClientLaunchInDevelopingMode();
    /// <summary>
    /// Code in Testing Mode will only run at the testing phrase of the project
    /// </summary>
    protected abstract void ClientLaunchInTestingMode();
    /// <summary>
    /// Code in Released Mode will only run at the released phrase of the project
    /// </summary>
    protected abstract void ClientLaunchInReleasedMode();


    /// <summary>
    /// Code in Developing Mode will only run at the development phrase of the project
    /// </summary>
    protected abstract void ServerLaunchInDevelopingMode();
    /// <summary>
    /// Code in Testing Mode will only run at the testing phrase of the project
    /// </summary>
    protected abstract void ServerLaunchInTestingMode();
    /// <summary>
    /// Code in Released Mode will only run at the released phrase of the project
    /// </summary>
    protected abstract void ServerLaunchInReleasedMode();

}

