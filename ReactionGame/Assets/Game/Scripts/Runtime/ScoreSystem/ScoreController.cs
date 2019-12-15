using GameplayEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.MessageSystem;

public class ScoreController : MonoBehaviour
{
    private int score;
    private float startTime;

    private void OnEnable()
    {
        Messenger m = Messenger.Default;
        m.AddListener<Answered>(OnAnswered);
        m.AddListener<TableReady>(e => StartTimer());
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
            score += 1;
        }
        float reactionTime = Time.time - startTime;

        Debug.Log($"Score = {score} with time of {reactionTime}");
    }
}
