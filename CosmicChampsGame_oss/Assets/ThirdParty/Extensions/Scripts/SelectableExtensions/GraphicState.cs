using System;
using DG.Tweening;
using UnityEngine;

namespace ThirdParty.Extensions.SelectableExtensions
{
    [Serializable]
    public class GraphicState
    {
        [SerializeField]
        private UnityEngine.UI.Graphic _graphic;

        [SerializeField]
        private Color _color = Color.white;

        public void Apply (float duration)
        {
            _graphic.DOKill ();
            _graphic.DOColor (_color, duration);
        }
    }
}