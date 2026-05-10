using ThirdParty.Extensions.Attributes;
using ThirdParty.Extensions.SelectableExtensions;
using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions.ToggleExtensions
{
    public class ToggleState : MonoBehaviour
    {
        [SerializeField, ArrayElementTitle ("image")]
        private ImageState[] images;

        [SerializeField, ArrayElementTitle ("graphic")]
        private GraphicState[] graphics;

        [SerializeField]
        private float duration;

        protected Toggle Toggle { private set; get; }

        protected virtual void Awake ()
        {
            Toggle = GetComponent<Toggle> ();
        }

        protected void Apply (float duration)
        {
            foreach (var image in images)
            {
                image.Apply (duration);
            }

            foreach (var graphic in graphics)
            {
                graphic.Apply (duration);
            }
        }

        protected void Apply ()
        {
            Apply (duration);
        }
    }
}