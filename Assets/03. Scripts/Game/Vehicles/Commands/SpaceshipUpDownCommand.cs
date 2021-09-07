using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Architecture;
using MikroFramework.Pool;

public class SpaceshipUpDownCommand : AbstractCommand<SpaceshipUpDownCommand> {

    private SpaceshipModel model;
    private float axis;

    public static SpaceshipUpDownCommand Allocate(SpaceshipModel spaceshipModel, float axis)
    {
        SpaceshipUpDownCommand spaceshipAccelerateCommand =
            SafeObjectPool<SpaceshipUpDownCommand>.Singleton.Allocate();

        spaceshipAccelerateCommand.model = spaceshipModel;
        spaceshipAccelerateCommand.axis = axis;

        return spaceshipAccelerateCommand;
    }

    protected override void OnExecute() {
        model.AddUpDownSpeed(axis);
    }
}

