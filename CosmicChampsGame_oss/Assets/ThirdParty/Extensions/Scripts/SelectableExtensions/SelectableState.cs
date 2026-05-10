using ThirdParty.Extensions.Attributes;
using ThirdParty.Extensions.Components;
using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions.SelectableExtensions
{
    [RequireComponent (typeof (Selectable))]
    public class SelectableState : MonoBehaviour
    {
        [SerializeField, ArrayElementTitle ("_image")]
        private ImageState[] _images;

        [SerializeField, ArrayElementTitle ("_graphic")]
        private GraphicState[] _graphics;

        [SerializeField, ArrayElementTitle ("_text")]
        private TextMeshPROState[] _texts;

        [SerializeField]
        private GraphicsPressScaleAnimation _pressScaleAnimation;

        [SerializeField]
        private bool _pressScaleAnimationState;

        [SerializeField]
        private float _duration;

        protected Selectable Selectable { private set; get; }

        protected virtual void Awake ()
        {
            Selectable = GetComponent<Selectable> ();
        }

        protected void Apply (float duration)
        {
            foreach (var image in _images)
            {
                image.Apply (duration);
            }

            foreach (var graphic in _graphics)
            {
                graphic.Apply (duration);
            }

            foreach (var text in _texts)
            {
                text.Apply (duration);
            }

            if (_pressScaleAnimation)
                _pressScaleAnimation.enabled = _pressScaleAnimationState;
        }

        protected void Apply ()
        {
            Apply (_duration);
        }
    }
}