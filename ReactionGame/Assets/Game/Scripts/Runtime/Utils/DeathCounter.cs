using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wokarol.MessageSystem;
using DG.Tweening;

public class DeathCounter : MonoBehaviour
{
    [SerializeField, Range(200, 5000)] private int deathTimeInMS = 1500;
    [SerializeField] private Image stateDisplay = null;

    private bool timerActive = false;
    private bool wasTriggered = false;
    private float startTime;

    private bool gameActive = true;

    private void OnEnable()
    {
        Messenger.Default.AddListener<GameplayEvents.TableReady>(_ => StartTimer());
        Messenger.Default.AddListener<GameplayEvents.Answered>(_ => ClearTimer());
        Messenger.Default.AddListener<GameplayEvents.GameOver>(e => gameActive = false);
    }

    private void OnDisable()
    {
        Messenger.Default.RemoveAllListenersFor(this);
    }

    private void Start()
    {
        stateDisplay.fillAmount = 0;
    }

    private void Update()
    {
        if (!timerActive || !gameActive)
            return;

        float timePassed = (Time.time - startTime) * 1000f;
        float timerState = Mathf.InverseLerp(0, deathTimeInMS, timePassed);

        stateDisplay.fillAmount = timerState;

        if(!wasTriggered && timerState >= 1) {
            wasTriggered = true;
            Messenger.Default.SendMessage(new GameplayEvents.DeathTimePassed());
        }
    }

    private void StartTimer()
    {
        startTime = Time.time;
        timerActive = true;
        wasTriggered = false;
    }

    private void ClearTimer()
    {
        timerActive = false;
        wasTriggered = false;
        stateDisplay.DOFillAmount(0, 0.075f);
    }
}
