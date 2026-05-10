using UnityEngine;
using Zenject;

namespace CosmicChamps.UI
{
    public class Disabler : IInitializable
    {
        public void Initialize ()
        {
            var objects = GameObject.FindGameObjectsWithTag (Tags.DisableOnAwake);
            foreach (var o in objects)
            {
                o.SetActive (false);
            }
        }
    }
}