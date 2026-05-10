using UnityEngine;

namespace ThirdParty.Extensions
{
    public static class ColorExtensions
    {
        public static Color WithA (this Color color, float a)
        {
            return new Color (color.r, color.g, color.b, a);
        }
    }
}