using UnityEngine;

namespace CosmicChamps.Utils
{
    public class DebugDrawer
    {
        public static void DrawCircle (Vector3 center, float radius, Plane plane, Color color, float duration)
        {
            const int iterations = 100;
            for (var i = 1; i < iterations; i++)
            {
                var angleA = Mathf.PI * 2 / iterations * (i - 1);
                var angleB = Mathf.PI * 2 / iterations * i;

                Debug.DrawLine (
                    center + new Vector3 (Mathf.Cos (angleA), 0f, Mathf.Sin (angleA)) * radius,
                    center + new Vector3 (Mathf.Cos (angleB), 0f, Mathf.Sin (angleB)) * radius,
                    color,
                    duration);
            }
        }
    }
}