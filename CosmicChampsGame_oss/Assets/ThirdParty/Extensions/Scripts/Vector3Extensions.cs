using UnityEngine;

namespace ThirdParty.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 WithComponent (this Vector3 vector3, int componentIndex, float value)
        {
            vector3[componentIndex] = value;
            return vector3;
        }

        public static Vector3 WithX (this Vector3 vector3, float x)
        {
            return WithComponent (vector3, 0, x);
        }

        public static Vector3 WithY (this Vector3 vector3, float y)
        {
            return WithComponent (vector3, 1, y);
        }

        public static Vector3 WithZ (this Vector3 vector3, float z)
        {
            return WithComponent (vector3, 2, z);
        }
        
        public static Vector3 AddX (this Vector3 vector3, float x)
        {
            return WithComponent (vector3, 0, vector3.x + x);
        }

        public static Vector3 AddY (this Vector3 vector3, float y)
        {
            return WithComponent (vector3, 1, vector3.y + y);
        }

        public static Vector3 AddZ (this Vector3 vector3, float z)
        {
            return WithComponent (vector3, 2, vector3.z + z);
        }
    }
}