using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

using DG.Tweening;

public class HeartContainer : MonoBehaviour
{
    [SerializeField] private Image fillImage = null;
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private Ease fadeEase = Ease.InQuint;

    private bool state = true;

    public void SetState(bool v)
    {
        if (state == v) return;

        if (v) {
            ResetHeart();
        } else {
            FadeHeart();
        }
    }

    [Button("Fade Out", ButtonAttribute.EnableMode.Playmode)]
    private void FadeHeart()
    {
        state = false;
        fillImage.DOFade(0, fadeDuration).SetEase(fadeEase);
    }

    [Button("Fade In", ButtonAttribute.EnableMode.Playmode)]
    private void ResetHeart()
    {
        state = true;
        fillImage.DOFade(1, fadeDuration).SetEase(fadeEase);
    }
}
