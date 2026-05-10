using UnityEngine;

namespace CosmicChamps.UI
{
    public abstract class Progressbar : MonoBehaviour
    {
        public abstract void SetValue (float value, bool immediate = false);
    }
}