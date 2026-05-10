using System;
using DG.Tweening;
using ThirdParty.Extensions;
using ThirdParty.Extensions.CanvasGroupFader;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CosmicChamps.UI
{
    public class ProgressIcon : MonoBehaviour
    {
        [FormerlySerializedAs ("graphic")]
        [SerializeField]
        private Graphic _graphic;

        [FormerlySerializedAs ("loopDuration")]
        [SerializeField]
        private float _loopDuration;

        [FormerlySerializedAs ("fadeDuration")]
        [SerializeField]
        private float _fadeDuration;

        [FormerlySerializedAs ("disabledByDefault")]
        [SerializeField]
        private bool _disabledByDefault = true;

        private Component _fadeComponent;

        private void Awake ()
        {
            _fadeComponent = _graphic.gameObject == gameObject ? _graphic : transform;
        }

        private void Start ()
        {
            if (_disabledByDefault)
                DoFadeOut (0f);
        }

        private void DoFadeIn (float duration)
        {
            // Debug.Log ($"DoFadeIn {duration}", gameObject);
            if (_graphic.gameObject == gameObject)
            {
                _graphic.DOKill ();
                _graphic.DOFade (1f, duration);
            } else
            {
                _fadeComponent.FadeIn (duration);
            }
        }

        private void DoFadeOut (float duration)
        {
            // Debug.Log ($"DoFadeOut {duration}", gameObject);
            if (_graphic.gameObject == gameObject)
            {
                _graphic.DOKill ();
                _graphic.DOFade (0f, duration);
            } else
            {
                _fadeComponent.FadeOut (duration);
            }
        }

        private void DoKill ()
        {
            _graphic.DOKill ();
            if (_graphic.gameObject != gameObject)
                _fadeComponent.DOKill ();
        }

        private void OnEnable ()
        {
            _graphic.transform.localRotation = Quaternion.identity;
            DOTween
                .To (x => _graphic.transform.localEulerAngles = Vector3.zero.WithZ (x), 0f, -360f, _loopDuration)
                .SetLoops (-1)
                .SetEase (Ease.Linear)
                .SetId (this);
        }

        private void OnDisable ()
        {
            DoKill ();
            this.DOKill ();
        }

        public void FadeIn ()
        {
            DoKill ();
            DoFadeIn (_fadeDuration);
        }

        public void FadeOut (float duration)
        {
            DoKill ();
            DoFadeOut (duration);
        }

        public void FadeOut ()
        {
            DoKill ();
            DoFadeOut (_fadeDuration);
        }

        public void FadeOutImmediate ()
        {
            DoKill ();
            DoFadeOut (0f);
        }
    }
}