using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color normalTint = Color.white;
    [SerializeField] private Color inactiveTint = Color.black * Color.white;
    [Space]
    [SerializeField] private float transitionTime = 0.2f;
    [SerializeField] private bool fadePattern = true;
    [Header("Binds")]
    [SerializeField] private SpriteColorInjector borderColor;
    [SerializeField] private SpriteColorInjector patternColor;

    [SerializeField, HideInInspector] private float transitionSpeed;

    private float colorLerp = 1;
    private float targetColorLerp = 1;

    private void OnValidate()
    {
        transitionSpeed = 1 / transitionTime;
    }

    private void Update()
    {
        colorLerp = Mathf.MoveTowards(colorLerp, targetColorLerp, Time.deltaTime * transitionSpeed);
        Color newColor = Color.Lerp(inactiveTint, normalTint, colorLerp);
        borderColor.Tint = newColor;

        if (fadePattern)
            newColor.a = colorLerp;

        patternColor.Tint = newColor;
    }

    public void SetActive(bool v)
    {
        targetColorLerp = v ? 1 : 0;
    }

    public void SuddenSetActive(bool v)
    {
        targetColorLerp = colorLerp = v ? 1 : 0;
    }

    [ContextMenu("Set Active to TRUE")]
    private void SetActiveToTrue() => SetActive(true);
    [ContextMenu("Set Active to FALSE")]
    private void SetActiveToFalse() => SetActive(false);
}
