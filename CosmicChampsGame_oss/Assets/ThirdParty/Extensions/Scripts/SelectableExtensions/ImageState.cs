using System;
using DG.Tweening;
using UnityEngine;

namespace ThirdParty.Extensions.SelectableExtensions
{
    [Serializable]
    public class ImageState
    {
        [SerializeField]
        private UnityEngine.UI.Image _image;

        [SerializeField]
        private Sprite _sprite;

        [SerializeField]
        private Color _color = Color.white;

        public void Apply (float duration)
        {
            _image.DOKill ();
            _image.DOSpriteFade (_sprite, duration);
            _image.DOColor (_color, duration);
        }
    }
}