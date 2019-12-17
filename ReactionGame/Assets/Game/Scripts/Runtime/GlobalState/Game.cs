using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Game
{
    public static SessionEndData SessionEndData { get; } = new SessionEndData();
}

public class SessionEndData
{
    public int EndScore { get; private set; }

    public void FillEndData(int score)
    {
        Debug.Log($"Filled end data => score:{score}");
        EndScore = score;
    }
}