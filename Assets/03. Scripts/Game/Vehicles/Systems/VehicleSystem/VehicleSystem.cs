using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.DataStructures;
using MikroFramework.Singletons;
using MikroNHibernateCore;
using Mirror;
using UnityEngine;

public interface IVehicleSystem:ISystem {

}

public partial class VehicleSystem : AbstractNetworkedSystem, IVehicleSystem, ISingleton {
    

    public static VehicleSystem Singleton {
        get {
            return SingletonProperty<VehicleSystem>.Singleton;
        }
    }

    [ServerCallback]
    protected override void OnServerInit() {
        
    }

    [Command]
    private void CmdCreateNewVehicle(VehicleType vehicleType, int id) {
        GameObject createdVehicle = VehicleFactory.CreateVehicle(vehicleType, id);
        NetworkServer.Spawn(createdVehicle);
        
    } 

    void ISingleton.OnSingletonInit() { }
}
