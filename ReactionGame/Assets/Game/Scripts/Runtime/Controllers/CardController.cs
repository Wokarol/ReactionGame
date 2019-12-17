using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using DG.Tweening;
using NaughtyAttributes;

[RequireComponent(typeof(SortingGroup))]
public class CardController : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] private Color normalTint = Color.white;
    [SerializeField] private Color inactiveTint = Color.black * Color.white;
    [Space]
    [SerializeField] private float fadeIntoBackTime = 0.2f;
    [SerializeField] private float fadeOutTime = 0.2f;
    [SerializeField] private bool fadePattern = true;
    [Header("Binds")]
    [SerializeField] private SpriteColorInjector borderColor = null;
    [SerializeField] private SpriteColorInjector patternColor = null;
    [Space]
    [SerializeField] private SpriteRenderer patternRenderer = null;

    private SortingGroup sortingGroup = null;

    public event Action OnClicked;
    public int SoringOrder {
        get => sortingGroup
                ? sortingGroup.sortingOrder
                : 0;
        set {
            if (sortingGroup)
                sortingGroup.sortingOrder = value;
        }
    }

    private void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
    }

    public void SetCard(CardData data)
    {
        patternRenderer.sprite = data.Pattern;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
    }

    [Button("Reset fade", ButtonAttribute.EnableMode.Playmode)]
    public void ResetFade()
    {
        borderColor.Tint = normalTint;
        patternColor.Tint = normalTint;
    }

    [Button("Fade into back", ButtonAttribute.EnableMode.Playmode)]
    public void FadeIntoBack() => FadeIntoBack(1);

    public void FadeIntoBack(float scale)
    {
        borderColor.DOTint(inactiveTint, fadeIntoBackTime * scale);

        Color targetPatternTint = inactiveTint;
        if (fadePattern)
            targetPatternTint.a = 0;
        patternColor.DOTint(targetPatternTint, fadeIntoBackTime * scale);
    }

    [Button("Fade out", ButtonAttribute.EnableMode.Playmode)]
    public void FadeOut() => FadeOut(1);

    public void FadeOut(float scale)
    {
        borderColor.DOAlpha(0, fadeOutTime * scale);
        patternColor.DOAlpha(0, fadeOutTime * scale);
    }
}
