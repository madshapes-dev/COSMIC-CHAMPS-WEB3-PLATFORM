using System;
using DG.Tweening;
using ThirdParty.Extensions;
using UnityEngine;

namespace CosmicChamps.UI
{
    public class RectTransformAnchorProgressbar : Progressbar
    {
        private enum Anchor
        {
            Min,
            Max
        }

        [SerializeField]
        private RectTransform _bar;

        [SerializeField]
        private float _valueChangeDuration = 0.2f;

        [SerializeField]
        private Anchor _anchor;

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
                switch (_anchor)
                {
                    case Anchor.Min:
                        _bar.anchorMin = _bar.anchorMin.WithX (value);
                        break;
                    case Anchor.Max:
                        _bar.anchorMax = _bar.anchorMax.WithX (value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException ();
                }
            else
                switch (_anchor)
                {
                    case Anchor.Min:
                        _tween = _bar.DOAnchorMin (_bar.anchorMin.WithX (value), _valueChangeDuration);
                        break;
                    case Anchor.Max:
                        _tween = _bar.DOAnchorMax (_bar.anchorMax.WithX (value), _valueChangeDuration);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException ();
                }
        }
    }
}