using System;
using UnityEngine;

namespace ThirdParty.Extensions
{
    public static class RectTransformExtensions
    {
        private static readonly Vector3[] _worldCorners = new Vector3[4];
        private static Camera _mainCamera;

        public static void AlignToWorldPosition (this RectTransform rectTransform, Vector3 worldPosition, Camera camera = null)
        {
            if (camera == null)
            {
                if (_mainCamera == null)
                    _mainCamera = Camera.main;

                camera = _mainCamera;
            }

            if (camera == null)
                throw new InvalidOperationException ("No camera found");

            var screenPoint = camera.WorldToScreenPoint (worldPosition);
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle (
                    rectTransform.parent as RectTransform,
                    screenPoint,
                    null,
                    out var localPosition))

                return;

            rectTransform.localPosition = localPosition;
        }

        public static Rect GetWorldRect (this RectTransform rectTransform)
        {
            rectTransform.GetWorldCorners (_worldCorners);

            var minX = Mathf.Min (_worldCorners[0].x, _worldCorners[1].x, _worldCorners[2].x, _worldCorners[3].x);
            var maxX = Mathf.Max (_worldCorners[0].x, _worldCorners[1].x, _worldCorners[2].x, _worldCorners[3].x);
            var minY = Mathf.Min (_worldCorners[0].y, _worldCorners[1].y, _worldCorners[2].y, _worldCorners[3].y);
            var maxY = Mathf.Max (_worldCorners[0].y, _worldCorners[1].y, _worldCorners[2].y, _worldCorners[3].y);

            return Rect.MinMaxRect (minX, minY, maxX, maxY);
        }

        public static void Align (this RectTransform rectTransform, RectTransform alignTo, bool alignSize = true)
        {
            alignTo.GetWorldCorners (_worldCorners);

            var leftBottomScreen = RectTransformUtility.WorldToScreenPoint (null, _worldCorners[0]);
            var rightTopScreen = RectTransformUtility.WorldToScreenPoint (null, _worldCorners[2]);

            Vector2 leftBottom, rightTop;

            var parent = rectTransform.parent as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle (parent, leftBottomScreen, null, out leftBottom);
            RectTransformUtility.ScreenPointToLocalPointInRectangle (parent, rightTopScreen, null, out rightTop);

            var rect = Rect.MinMaxRect (leftBottom.x, leftBottom.y, rightTop.x, rightTop.y);
            rectTransform.localPosition = rect.center;

            if (!alignSize)
                return;

            rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, rect.size.x);
            rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, rect.size.y);
        }

        public static bool InBounds (this RectTransform rectTransform, RectTransform bounds)
        {
            var rect = GetWorldRect (rectTransform);
            var bordersRect = GetWorldRect (bounds);
            const float eps = 1f;

            return rect.xMin.CompareWithEps (bordersRect.xMin, eps) >= 0 &&
                   rect.yMin.CompareWithEps (bordersRect.yMin, eps) >= 0 &&
                   rect.xMax.CompareWithEps (bordersRect.xMax, eps) <= 0 &&
                   rect.yMax.CompareWithEps (bordersRect.yMax, eps) <= 0;
        }

        public static bool InScreenBounds (this RectTransform rectTransform)
        {
            var rect = GetWorldRect (rectTransform);
            const float eps = 1f;

            return rect.xMin.CompareWithEps (0f, eps) >= 0 &&
                   rect.yMin.CompareWithEps (0f, eps) >= 0 &&
                   rect.xMax.CompareWithEps (Screen.width, eps) <= 0 &&
                   rect.yMax.CompareWithEps (Screen.height, eps) <= 0;
        }
    }
}