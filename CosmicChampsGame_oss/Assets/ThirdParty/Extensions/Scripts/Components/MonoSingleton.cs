using UnityEngine;

namespace ThirdParty.Extensions.Components
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T Instance { private set; get; }

        protected virtual void Awake ()
        {
            Instance = this as T;
        }
    }
}