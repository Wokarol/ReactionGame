using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace DG.Tweening
{
    public static class DOTweenModuleColor
    {
        public static TweenerCore<Color, Color, ColorOptions> DOTint(this SpriteColorInjector target, Color endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = DOTween.To(() => target.Tint, x => target.Tint = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }

        public static TweenerCore<float, float, FloatOptions> DOAlpha(this SpriteColorInjector target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.Tint.a, v => {
                Color c = target.Tint;
                c.a = v;
                target.Tint = c;
            }, endValue, duration);

            t.SetTarget(target);
            return t;
        }
    }
}
