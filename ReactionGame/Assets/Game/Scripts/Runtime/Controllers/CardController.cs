using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] private Color normalTint = Color.white;
    [SerializeField] private Color inactiveTint = Color.black * Color.white;
    [Space]
    [SerializeField] private float transitionTime = 0.2f;
    [SerializeField] private bool fadePattern = true;
    [Header("Binds")]
    [SerializeField] private SpriteColorInjector borderColor = null;
    [SerializeField] private SpriteColorInjector patternColor = null;
    [Space]
    [SerializeField] private SpriteRenderer patternRenderer = null;

    [SerializeField, HideInInspector] private float transitionSpeed;

    private float colorLerp = 1;
    private float targetColorLerp = 1;

    public event Action OnClicked;

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

    public void SetCard(CardData data)
    {
        patternRenderer.sprite = data.Pattern;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
    }

    [ContextMenu("Set Active to TRUE")]
    private void SetActiveToTrue() => SetActive(true);
    [ContextMenu("Set Active to FALSE")]
    private void SetActiveToFalse() => SetActive(false);
}
