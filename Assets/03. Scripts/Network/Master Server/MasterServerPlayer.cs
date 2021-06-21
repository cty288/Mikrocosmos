using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MasterServerPlayer : NetworkBehaviour
{
    [ClientRpc]
    public void RpcTest() {
        print("233");
    }
}
