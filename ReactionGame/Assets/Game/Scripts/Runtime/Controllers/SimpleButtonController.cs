using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleButtonController : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text textMesh = null;

    public event Action OnClicked;
    public string Text {
        get => textMesh.text;
        set => textMesh.text = value;
    }

    public void Perform_Clicked()
    {
        OnClicked?.Invoke();
    }
}
