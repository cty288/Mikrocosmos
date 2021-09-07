using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

public class SpaceshipYawRotateCommand:AbstractCommand<SpaceshipYawRotateCommand> {
    private float axis;
    private SpaceshipModel model;

    public static SpaceshipYawRotateCommand Allocate(float axis, SpaceshipModel model) {
        SpaceshipYawRotateCommand command = SafeObjectPool<SpaceshipYawRotateCommand>.Singleton.Allocate();
        command.axis = axis;
        command.model = model;
        return command;
    }

    protected override void OnExecute() {
        model.AddAngularSpeed(axis);
    }
}

