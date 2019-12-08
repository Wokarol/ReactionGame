using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteColorInjector : MonoBehaviour
{
    [SerializeField] private ColorConfig.Colors color = ColorConfig.Colors.Background;
    [SerializeField] private Color tint = Color.white;

    private new SpriteRenderer renderer;

    public Color Tint {
        get => tint;
        set {
            tint = value;
            SetColor();
        }
    }

    private void OnValidate()
    {
        SetColor();
    }

    private void Awake()
    {
        Configuration.Get<ColorConfig>().ColorsChanged += SetColor;
    }

    private void OnDestroy()
    {
        Configuration.Get<ColorConfig>().ColorsChanged -= SetColor;
    }

    void SetColor()
    {
        if (!renderer) {
            renderer = GetComponent<SpriteRenderer>();
        }
        if (renderer) {
            renderer.color = Configuration.Get<ColorConfig>().GetColor(color) * tint;
        }
    }
}
