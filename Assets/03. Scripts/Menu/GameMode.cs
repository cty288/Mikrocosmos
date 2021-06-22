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

    public Mode getGameMode() {
        return mode;
    }

    public int GetRequiredPlayerNumber() {
        return requiredPlayerNumber;
    }

    public int GetTeamNumber() {
        return teamNumber;
    }
}

public class QuickMode : GameMode {
    public QuickMode() {
        mode = Mode.Fast;
        requiredPlayerNumber = 10;
        teamNumber = 2;
    }
}

public class StandardMode : GameMode {
    public StandardMode() {
        mode = Mode.Standard;
        requiredPlayerNumber = 10;
        teamNumber = 2;
    }
}


public class MarrathonMode : GameMode
{
    public MarrathonMode()
    {
        mode = Mode.Marathon;
        requiredPlayerNumber = 10;
        teamNumber = 2;
    }
}