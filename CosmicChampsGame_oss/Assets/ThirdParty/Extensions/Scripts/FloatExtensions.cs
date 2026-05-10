using UnityEngine;

namespace ThirdParty.Extensions
{
    public static class FloatExtensions
    {
        private const float Eps = 0.001f;

        public static bool EqualsWithEps (this float f, float another)
        {
            return f.CompareWithEps (another, Eps) == 0;
        }

        public static bool EqualsWithEps (this float f, float another, float eps)
        {
            return f.CompareWithEps (another, eps) == 0;
        }

        public static int CompareWithEps (this float f, float another)
        {
            return f.CompareWithEps (another, Eps);
        }

        public static int CompareWithEps (this float f, float another, float eps)
        {
            var delta = f - another;
            if (Mathf.Abs (f - another) <= eps)
                return 0;

            return delta > 0 ? 1 : -1;
        }
    }
}