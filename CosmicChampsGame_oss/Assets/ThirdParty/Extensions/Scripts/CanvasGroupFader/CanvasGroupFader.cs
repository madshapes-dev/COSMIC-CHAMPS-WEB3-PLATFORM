using DG.Tweening;
using UnityEngine;

namespace ThirdParty.Extensions.CanvasGroupFader
{
    [RequireComponent (typeof (CanvasGroup))]
    public class CanvasGroupFader : MonoBehaviour
    {
        [SerializeField]
        private float _duration = 0.2f;

        private CanvasGroup _canvasGroup;
        private Tween _tween;

        private CanvasGroup GetCanvasGroup ()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup> ();

            return _canvasGroup;
        }

        public Tween FadeIn ()
        {
            return FadeIn (_duration);
        }

        public Tween FadeIn (float duration)
        {
            _tween?.Kill ();

            var canvasGroup = GetCanvasGroup ();
            canvasGroup.DOKill ();

            canvasGroup.alpha = this.IsVisible () ? canvasGroup.alpha : 0f;
            this.SetVisible (true);
            
            return _tween = DOTween
                .To (x => canvasGroup.alpha = x, canvasGroup.alpha, 1f, duration)
                .SetEase (Ease.Linear);
        }

        public Tween FadeOut ()
        {
            return FadeOut (_duration);
        }

        public Tween FadeOut (float duration)
        {
            _tween?.Kill ();

            var canvasGroup = GetCanvasGroup ();
            canvasGroup.DOKill ();

            canvasGroup.alpha = 1f;
            return _tween = GetCanvasGroup ()
                .DOFade (0f, duration * _canvasGroup.alpha)
                .SetEase (Ease.Linear)
                .OnComplete (() => this.SetVisible (false));
        }
    }
}