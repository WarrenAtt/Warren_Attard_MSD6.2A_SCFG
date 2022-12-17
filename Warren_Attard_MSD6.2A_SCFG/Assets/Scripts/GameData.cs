using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    private static bool isPlayerRunning = false;

    public static bool IsPlayerRunning
    {
        set { isPlayerRunning = value; }
        get { return isPlayerRunning; }
    }

    private static float health = 100f;

    public static float Health
    {
        set { health = value; }
        get { return health; }
    }

    private static float score = 0;

    public static float Score
    {
        set { score = value; }
        get { return score; }
    }

    public enum Difficuly
    {
        Easy,
        Normal,
        Hard
    }

    public static Difficuly SelectedDifficuly;
}
