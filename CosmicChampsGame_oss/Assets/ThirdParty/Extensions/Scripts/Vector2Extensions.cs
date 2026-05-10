using UnityEngine;

namespace ThirdParty.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 WithComponent (this Vector2 vector2, int componentIndex, float value)
        {
            vector2[componentIndex] = value;
            return vector2;
        }

        public static Vector2 AddX (this Vector2 vector2, float x)
        {
            return WithComponent (vector2, 0, vector2.x + x);
        }

        public static Vector2 AddY (this Vector2 vector2, float y)
        {
            return WithComponent (vector2, 1, vector2.y + y);
        }

        public static Vector2 WithX (this Vector2 vector2, float x)
        {
            return WithComponent (vector2, 0, x);
        }

        public static Vector2 WithY (this Vector2 vector2, float y)
        {
            return WithComponent (vector2, 1, y);
        }
    }
}