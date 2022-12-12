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

    public enum Difficuly
    {
        Easy,
        Normal,
        Hard
    }

    public static Difficuly SelectedDifficuly;
}
