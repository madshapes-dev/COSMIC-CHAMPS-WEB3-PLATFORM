using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicChamps.UI
{
    public class ImageFillProgressbar : Progressbar
    {
        [SerializeField]
        private Image _bar;

        [SerializeField]
        private float _valueChangeDuration = 0.2f;

        private Tween _tween;

        private void OnDestroy ()
        {
            KillTween ();
        }

        private void KillTween ()
        {
            _tween?.Kill ();
            _tween = null;
        }

        public override void SetValue (float value, bool immediate = false)
        {
            KillTween ();

            if (immediate)
                _bar.fillAmount = value;
            else
                _tween = _bar.DOFillAmount (value, _valueChangeDuration);
        }
    }
}