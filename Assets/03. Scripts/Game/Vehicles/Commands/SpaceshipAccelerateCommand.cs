using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

public class SpaceshipAccelerateCommand : AbstractCommand<SpaceshipAccelerateCommand> {
    public SpaceshipModel SpaceshipModel;
    public bool accelerate = true;

    public static SpaceshipAccelerateCommand Allocate(SpaceshipModel spaceshipModel,bool accelerate) {
        SpaceshipAccelerateCommand spaceshipAccelerateCommand =
            SafeObjectPool<SpaceshipAccelerateCommand>.Singleton.Allocate();

        spaceshipAccelerateCommand.SpaceshipModel = spaceshipModel;
        spaceshipAccelerateCommand.accelerate = accelerate;

        return spaceshipAccelerateCommand;
    }

    protected override void OnExecute() {
        SpaceshipModel.Accelerate(accelerate);
    }
}
