using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public class SeriesC_Spaceship : Spaceship {
    
    public override void Attack() {
        
    }

    public override string GenerateName() {
        return "";
    }

    [ClientCallback]
    protected override void ClientInit() {
        
    }


    [ServerCallback]
    protected override void ServerInit() {

    }
}
