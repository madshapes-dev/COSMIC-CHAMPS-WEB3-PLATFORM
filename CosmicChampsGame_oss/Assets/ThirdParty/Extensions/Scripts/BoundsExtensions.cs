using UnityEngine;

namespace ThirdParty.Extensions
{
    public static class BoundsExtensions
    {
        public static Bounds Transform (this Bounds bounds, Matrix4x4 matrix)
        {
            var min = matrix.MultiplyPoint3x4 (bounds.min);
            var max = matrix.MultiplyPoint3x4 (bounds.max);
            var newBounds = new Bounds (min, Vector3.zero);
            newBounds.Encapsulate (max);

            return newBounds;
        }

        public static Vector3 GetRandomPosition (this Bounds bounds) =>
            new(
                Random.Range (bounds.min.x, bounds.max.x),
                Random.Range (bounds.min.y, bounds.max.y),
                Random.Range (bounds.min.z, bounds.max.z));

        public static void Draw (this Bounds bounds, Color color, float duration)
        {
            void DrawRect (Vector3 min, Vector3 max)
            {
                Debug.DrawLine (min, min.WithX (max.x), color, duration);
                Debug.DrawLine (min.WithX (max.x), max, color, duration);
                Debug.DrawLine (max, min.WithZ (max.z), color, duration);
                Debug.DrawLine (min.WithZ (max.z), min, color, duration);
            }

            var minBottom = bounds.min;
            var maxBottom = bounds.max.WithY (bounds.min.y);

            var minTop = bounds.min.WithY (bounds.max.y);
            var maxTop = bounds.max;

            DrawRect (minBottom, maxBottom);
            DrawRect (minTop, maxTop);

            Debug.DrawLine (minBottom, minTop, color, duration);
            Debug.DrawLine (maxBottom, maxTop, color, duration);
            Debug.DrawLine (minBottom.WithX (maxBottom.x), minTop.WithX (maxTop.x), color, duration);
            Debug.DrawLine (minBottom.WithZ (maxBottom.z), minTop.WithZ (maxTop.z), color, duration);
        }
    }
}