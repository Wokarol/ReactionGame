using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathCounterDisplay : MonoBehaviour
{
    int blendNameHash = -1;

    [SerializeField] private Image stateDisplay = null;
    [SerializeField] private Animator animator;
    [SerializeField] private string blendName;

    private float currentValue;

    public float Value {
        get => currentValue;
        set {
            currentValue = value;
            stateDisplay.fillAmount = value;
            animator.SetFloat(blendNameHash, value);
        }
    }

    private void OnValidate()
    {
        blendNameHash = Animator.StringToHash(blendName);
    }

    private void Awake()
    {
        blendNameHash = Animator.StringToHash(blendName);
    }


    public void DOChange(float value, float duration)
    {
        DOVirtual.Float(currentValue, value, duration, v => Value = v);
    }
}
