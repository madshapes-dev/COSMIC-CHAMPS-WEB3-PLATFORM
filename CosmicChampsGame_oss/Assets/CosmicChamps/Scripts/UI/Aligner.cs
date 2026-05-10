using UnityEngine;
using UnityEngine.Serialization;

namespace CosmicChamps.UI
{
    [RequireComponent (typeof (RectTransform))]
    public class Aligner : MonoBehaviour
    {
        [FormerlySerializedAs ("offsetMin")]
        [SerializeField]
        private Vector2 _offsetMin;

        [FormerlySerializedAs ("offsetMax")]
        [SerializeField]
        private Vector2 _offsetMax;

        private void Awake ()
        {
            var rectTransform = GetComponent<RectTransform> ();
            rectTransform.offsetMin = _offsetMin;
            rectTransform.offsetMax = _offsetMax;
        }
    }
}