using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
    Fast,
    Standard,
    Marathon
}

public abstract class GameMode {
    protected Mode mode;
    protected int requiredPlayerNumber;
    protected int teamNumber;
    protected string queueName;

    public Mode getGameMode() {
        return mode;
    }

    public int GetRequiredPlayerNumber() {
        return requiredPlayerNumber;
    }

    public int GetTeamNumber() {
        return teamNumber;
    }

    public string GetQueueName() {
        return queueName;
    }

    /// <summary>
    /// Return a Gamemode obejct by Mode enum
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static GameMode GetGameModeObj(Mode mode) {
        switch (mode) {
            case Mode.Fast:
                return new QuickMode();
            case Mode.Standard:
                return new StandardMode();
            case Mode.Marathon:
                return new MarathonMode();
            default:
                return new StandardMode();
        }
    }
}

public class QuickMode : GameMode {
    public QuickMode() {
        mode = Mode.Fast;
        requiredPlayerNumber = 10;
        teamNumber = 2;
        queueName = "Quick_Mode";
    }
}

public class StandardMode : GameMode {
    public StandardMode() {
        mode = Mode.Standard;
        requiredPlayerNumber = 10;
        teamNumber = 2;
        queueName = "Standard_Mode";
    }
}


public class MarathonMode : GameMode
{
    public MarathonMode()
    {
        mode = Mode.Marathon;
        requiredPlayerNumber = 10;
        teamNumber = 2;
        queueName = "Marathon_Mode";
    }
}