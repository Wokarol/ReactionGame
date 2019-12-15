using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private string scoreFormat = "0 000 000";
    [SerializeField] private string deltaFormat = "000";
    [Space]
    [SerializeField] private TMPro.TMP_Text display = null;
    [SerializeField] private TMPro.TMP_Text deltaDisplay = null;
    [SerializeField] private ScoreController scoreController = null;

    private void OnValidate()
    {
        if (!display) 
            display = GetComponent<TMPro.TMP_Text>();
    }

    private void OnEnable()
    {
        scoreController.PointsChanged += OnPointsChanged;
    }
    private void OnDisable()
    {
        scoreController.PointsChanged -= OnPointsChanged;
    }


    private void OnPointsChanged(int delta)
    {
        display.text = scoreController.Score.ToString(scoreFormat);
        if (deltaDisplay)
            deltaDisplay.text = delta.ToString(deltaFormat);
    }
}
