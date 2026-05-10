using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ThirdParty.Extensions.SelectableExtensions
{
    [Serializable]
    public class TextMeshPROState
    {
        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        private TMP_ColorGradient _gradient;

        [SerializeField]
        private Color _color = Color.white;

        public void Apply (float duration)
        {
            _text.DOKill ();
            _text.DOColor (_color, duration);
            _text.colorGradientPreset = _gradient;
        }
    }
}