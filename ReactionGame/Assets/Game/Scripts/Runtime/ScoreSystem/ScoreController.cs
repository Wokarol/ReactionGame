using GameplayEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.MessageSystem;

public class ScoreController : MonoBehaviour
{
    [Tooltip("Score (Y) based on reaction time in ms (X)")]
    [SerializeField] private AnimationCurve scoreByTime = AnimationCurve.Linear(200, 150, 1000, 20);
    [SerializeField] private AnimationCurve timeScaleByScore = AnimationCurve.Linear(0, 1, 10000, 3);
    [SerializeField] private int scoreRounding = 10;

    private float startTime;

    public event Action<int> PointsChanged;
    public int Score { get; private set; } = 0;

    private void Awake()
    {
        Time.timeScale = 1;
    }

    private void OnEnable()
    {
        Messenger m = Messenger.Default;
        m.AddListener<Answered>(OnAnswered);
        m.AddListener<TableReady>(e => StartTimer());
        m.AddListener<GameOver>(e => Game.SessionEndData.FillEndData(Score));
    }

    private void OnDisable()
    {
        Messenger.Default.RemoveAllListenersFor(this);
    }

    private void StartTimer()
    {
        startTime = Time.time;
    }

    private void OnAnswered(Answered e)
    {
        if (e.Correct) {
            float reactionTime = (Time.time - startTime) * 1000;
            float rawScore = scoreByTime.Evaluate(reactionTime);

            int gainedPoints = Mathf.CeilToInt(rawScore / scoreRounding) * scoreRounding;
            Score += gainedPoints;
            PointsChanged?.Invoke(gainedPoints);
        }

        Time.timeScale = timeScaleByScore.Evaluate(Score);
    }
}
