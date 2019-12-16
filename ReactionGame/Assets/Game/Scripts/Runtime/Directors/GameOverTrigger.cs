using System;
using System.Collections;
using System.Collections.Generic;
using GameplayEvents;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wokarol.MessageSystem;

public class GameOverTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private string triggerToCall = "";
    [Space]
    [SerializeField] private string targetScene = "";
    [Space]
    [SerializeField] private float gameOverDelay = 0;
    [SerializeField] private float sceneSwitchDelay = 0;

    private void OnEnable()
    {
        Messenger.Default.AddListener<GameOver>(OnGameOver);
    }

    private void OnDisable()
    {
        Messenger.Default.RemoveListener<GameOver>(OnGameOver);
    }

    private void OnGameOver(GameOver e)
    {
        StartCoroutine(GameOverCoroutine());
        
    }

    private IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(gameOverDelay);
        animator.SetTrigger(triggerToCall);
        yield return new WaitForSeconds(sceneSwitchDelay);
        SceneManager.LoadSceneAsync(targetScene);
    }
}
