using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Game
{
    public static SessionEndData SessionEndData { get; } = new SessionEndData();
}

public static class SaveTags
{
    public const string Highscore = "Highscore";
}

public class SessionEndData
{
    public int Score { get; private set; }

    public void FillEndData(int score)
    {
        Debug.Log($"Filled end data => score:{score}");
        Score = score;
    }
}