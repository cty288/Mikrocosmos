using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

public class SpaceshipAccelerateCommand : AbstractCommand<SpaceshipAccelerateCommand> {
    public SpaceshipModel SpaceshipModel;
    public float accelerateAmount = 0;

    public static SpaceshipAccelerateCommand Allocate(SpaceshipModel spaceshipModel, float addAmount) {
        SpaceshipAccelerateCommand spaceshipAccelerateCommand =
            SafeObjectPool<SpaceshipAccelerateCommand>.Singleton.Allocate();

        spaceshipAccelerateCommand.SpaceshipModel = spaceshipModel;
        spaceshipAccelerateCommand.accelerateAmount = addAmount;

        return spaceshipAccelerateCommand;
    }

    protected override void OnExecute() {
        //Debug.Log(SpaceshipModel==null);
        SpaceshipModel.AddAcceleration(accelerateAmount);
    }
}
