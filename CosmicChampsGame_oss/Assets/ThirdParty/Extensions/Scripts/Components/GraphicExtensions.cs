using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions.Components
{
    public static class GraphicExtensions
    {
        public static Tween PulseHalo (
            this Graphic graphic,
            float startSize,
            float startFadeSize,
            float finalSize,
            float duration)
        {
            graphic.rectTransform.sizeDelta = Vector2.one * startSize;
            graphic.color = graphic.color.WithA (1f);

            graphic.DOKill ();

            return DOTween
                .Sequence ()
                .Append (
                    graphic
                        .rectTransform
                        .DOSizeDelta (Vector2.one * startFadeSize, duration / 2f)
                        .SetEase (Ease.Linear))
                .Append (
                    graphic
                        .rectTransform
                        .DOSizeDelta (Vector2.one * finalSize, duration / 2f)
                        .SetEase (Ease.Linear))
                .Join (graphic.DOFade (0f, duration / 2f).SetEase (Ease.Linear))
                .SetId (graphic);
        }

        public static Tween Fade (this Graphic graphic, bool fadeIn, float duration) =>
            fadeIn
                ? graphic.FadeIn (duration)
                : graphic.FadeOut (duration);

        public static Tween Fade (this Graphic graphic, bool fadeIn) =>
            fadeIn
                ? graphic.FadeIn ()
                : graphic.FadeOut ();

        public static Tween FadeIn (this Graphic graphic, float duration)
        {
            graphic.SetVisible (true);
            return graphic
                .DOFade (1f, duration)
                .SetId (graphic);
        }

        public static Tween FadeIn (this Graphic graphic)
        {
            graphic.SetVisible (true);
            return graphic
                .DOFade (1f, 0f)
                .SetId (graphic);
        }

        public static Tween FadeOut (this Graphic graphic, float duration) => graphic
            .DOFade (0f, duration)
            .OnComplete (() => graphic.SetVisible (false))
            .SetId (graphic);

        public static Tween FadeOut (this Graphic graphic) => graphic
            .DOFade (0f, 0f)
            .OnComplete (() => graphic.SetVisible (false))
            .SetId (graphic);
    }
}