using DG.Tweening;
using UnityEngine;

namespace ThirdParty.Extensions.CanvasGroupFader
{
    public static class CanvasGroupComponentExtensions
    {
        private static CanvasGroupFader CanvasGroupFader (Component component)
        {
            var canvasGroupFader = component.GetComponent<CanvasGroupFader> ();
            if (canvasGroupFader == null)
                canvasGroupFader = component
                    .gameObject
                    .AddComponent<CanvasGroupFader> ();

            return canvasGroupFader;
        }

        public static Tween FadeIn<T> (this T component, bool immediate = false) where T : Component
        {
            var canvasGroupFader = CanvasGroupFader (component);
            return immediate ? canvasGroupFader.FadeIn (0f) : canvasGroupFader.FadeIn ();
        }

        public static Tween FadeIn<T> (this T component, float duration) where T : Component
        {
            return CanvasGroupFader (component).FadeIn (duration);
        }

        public static Tween FadeOut<T> (this T component, bool immediate = false) where T : Component
        {
            var canvasGroupFader = CanvasGroupFader (component);
            return immediate ? canvasGroupFader.FadeOut (0f) : canvasGroupFader.FadeOut ();
        }

        public static Tween FadeOut<T> (this T component, float duration) where T : Component
        {
            return CanvasGroupFader (component).FadeOut (duration);
        }
    }
}