using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wokarol;

public class GameOverUIBrain : MonoBehaviour
{
    private int coverTriggerHash = -1;

    [SerializeField] private Button playAgainButton = null;
    [SerializeField] private Button mainMenuButton = null;
    [Space]
    [SerializeField] private string gameScene = "";
    [SerializeField] private string menuScene = "";
    [Space]
    [SerializeField] private Animator transitionAnimator = null;
    [SerializeField] private string uiCoverTrigger = "";
    [SerializeField] private float animationTime = 0.2f;

    private bool inputLocked = false;

    void Start()
    {
        Time.timeScale = 1;

        playAgainButton.onClick.AddListener(OnPlayButton);
        mainMenuButton.onClick.AddListener(OnMainButton);

        coverTriggerHash = Animator.StringToHash(uiCoverTrigger);
    }

    private void OnPlayButton()
    {
        if (inputLocked)
            return;
        inputLocked = true;

        transitionAnimator.SetTrigger(coverTriggerHash);
        Scheduler.DelayCall(() => SceneManager.LoadScene(gameScene), animationTime);
    }

    private void OnMainButton()
    {
        if (inputLocked)
            return;
        inputLocked = true;


        transitionAnimator.SetTrigger(coverTriggerHash);
        Scheduler.DelayCall(() => SceneManager.LoadScene(menuScene), animationTime);
    }
}
