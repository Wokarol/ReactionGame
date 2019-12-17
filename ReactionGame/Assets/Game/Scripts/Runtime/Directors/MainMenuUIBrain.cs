using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wokarol;

public class MainMenuUIBrain : MonoBehaviour
{
    private int hideTriggerHash = -1;

    [SerializeField] private Button playButton = null;
    [SerializeField] private Button quitButton = null;
    [Space]
    [SerializeField] private string gameScene = "";
    [Space]
    [SerializeField] private Animator animator = null;
    [SerializeField] private string uiHideTrigger = "";
    [SerializeField] private float sceneSwitchDelay = 0.2f;

    private bool inputLocked = false;

    private void OnValidate()
    {
        hideTriggerHash = Animator.StringToHash(uiHideTrigger);
    }

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayButton);
        quitButton.onClick.AddListener(() => Application.Quit());

        hideTriggerHash = Animator.StringToHash(uiHideTrigger);
    }

    private void OnPlayButton()
    {
        if (inputLocked)
            return;
        inputLocked = true;

        animator.SetTrigger(hideTriggerHash);
        Scheduler.DelayCall(() => SceneManager.LoadScene(gameScene), sceneSwitchDelay);
    }
}
