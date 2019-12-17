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
    [Space]
    [SerializeField] private Image blastImage = null;
    [SerializeField] private float newScale = 2;
    [SerializeField] private float newRotation = 30;
    [SerializeField] private float blastDuration = 0.6f;

    private bool state = true;

    public void SetState(bool v)
    {
        if (state == v) return;

        if (v) {
            ResetHeart();
        } else {
            LoseHeart();
        }
    }

    [Button("Lose Heart", ButtonAttribute.EnableMode.Playmode)]
    private void LoseHeart()
    {
        state = false;

        fillImage.DOFade(0, fadeDuration)
            .SetEase(fadeEase)
            .SetUpdate(true);

        const Ease ease = Ease.InSine;
        blastImage.DOFade(0, blastDuration)
            .SetEase(ease)
            .SetUpdate(true);
        blastImage.transform.DOScale(newScale, blastDuration)
            .SetEase(ease)
            .SetUpdate(true);
        blastImage.transform.DORotate(Vector3.forward * newRotation, blastDuration)
            .SetEase(ease)
            .SetUpdate(true);
    }

    [Button("Fade In", ButtonAttribute.EnableMode.Playmode)]
    private void ResetHeart()
    {
        state = true;
        fillImage.DOFade(1, fadeDuration).SetEase(fadeEase);
    }
}
