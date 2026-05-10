using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions
{
    public static class ScrollRectExtensions
    {
        public static Tween VerticalScrollTo (this ScrollRect scrollRect, RectTransform target, float speedFactor = 1f)
        {
            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds (scrollRect.transform, target);
            var scrollHeight = scrollRect.content.rect.height - scrollRect.viewport.rect.height;
            var targetPos = Mathf.Min (Mathf.Abs (bounds.center.y), scrollHeight);
            var targetPosNormalized = 1f - targetPos / scrollHeight;

            return scrollRect.VerticalScrollTo (targetPosNormalized, speedFactor);
        }

        public static Tween VerticalScrollTo (this ScrollRect scrollRect, float targetPos, float speedFactor = 1f)
        {
            var diff = Mathf.Abs (scrollRect.verticalNormalizedPosition - targetPos);
            return scrollRect.DOVerticalNormalizedPos (targetPos, diff * speedFactor);
        }

        public static Tween VerticalScrollForVisibility (this ScrollRect scrollRect, RectTransform target, float speedFactor = 1f)
        {
            var viewport = scrollRect.viewport;
            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds (viewport, target);

            float? targetPos = default;
            if (bounds.min.y < viewport.rect.min.y)
                targetPos = viewport.rect.min.y - bounds.min.y;
            else if (bounds.max.y > viewport.rect.max.y) targetPos = viewport.rect.max.y - bounds.max.y;

            if (!targetPos.HasValue)
                return null;

            var scrollHeight = scrollRect.content.rect.height - scrollRect.viewport.rect.height;
            var targetPosNormalized = 1f - (scrollRect.content.anchoredPosition.y + targetPos.Value) / scrollHeight;

            return scrollRect.VerticalScrollTo (targetPosNormalized, speedFactor);
        }
        /*public static Tween VerticalScrollTo (this ScrollRect scrollRect, RectTransform target, float speedFactor = 1f)
        {
            var height = scrollRect.content.rect.height - scrollRect.viewport.rect.height;
            var targetPos = Mathf.Clamp01 ((target.anchoredPosition.y + height) / height);

            return scrollRect.VerticalScrollTo (targetPos, speedFactor);
        }

        public static Tween VerticalScrollTo (this ScrollRect scrollRect, float targetPos, float speedFactor = 1f)
        {
            var diff = Mathf.Abs (scrollRect.verticalNormalizedPosition - targetPos);
            return scrollRect.DOVerticalNormalizedPos (targetPos, diff * speedFactor);
        }

        public static Tween HorizontalScrollTo (this ScrollRect scrollRect, float targetPos, float speedFactor = 1f)
        {
            var diff = Mathf.Abs (scrollRect.horizontalNormalizedPosition - targetPos);
            return scrollRect.DOHorizontalNormalizedPos (targetPos, diff * speedFactor);
        }

        public static Tween CenterOnHorizontally (
            this ScrollRect scrollRect,
            RectTransform target,
            float speedFactor = 1f)
        {
            var width = scrollRect.content.rect.width - scrollRect.viewport.rect.width;

            RectTransformUtility.ScreenPointToLocalPointInRectangle (
                target.parent as RectTransform,
                new Vector2 (Screen.width / 2f, Screen.height / 2f),
                null,
                out var localPoint);

            return scrollRect.HorizontalScrollTo (
                scrollRect.horizontalNormalizedPosition + (target.anchoredPosition.x - localPoint.x) / width,
                speedFactor);
        }

        public static Tween CenterOnVertically (
            this ScrollRect scrollRect,
            RectTransform target,
            float speedFactor = 1f)
        {
            var width = scrollRect.content.rect.height - scrollRect.viewport.rect.height;

            RectTransformUtility.ScreenPointToLocalPointInRectangle (
                target.parent as RectTransform,
                new Vector2 (Screen.height / 2f, Screen.height / 2f),
                null,
                out var localPoint);

            return scrollRect.HorizontalScrollTo (
                scrollRect.verticalNormalizedPosition + (target.anchoredPosition.y - localPoint.y) / width,
                speedFactor);
        }*/
    }
}