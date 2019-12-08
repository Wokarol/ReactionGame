using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorConfig : Configuration
{
    public enum Colors
    {
        Background, Accents
    }

    [SerializeField] private Color background = Color.white;
    [SerializeField] private Color accents = Color.white;

    public event Action ColorsChanged;

    private void OnValidate()
    {
        ColorsChanged?.Invoke();
    }

    public Color GetColor(Colors color)
    {
        switch (color) {
            case Colors.Background: return background;
            case Colors.Accents: return accents;
            default: return Color.black;
        }
    }
}
