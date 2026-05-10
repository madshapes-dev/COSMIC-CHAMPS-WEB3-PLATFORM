using System;
using DG.Tweening;
using ThirdParty.Extensions;
using TMPro;
using UnityEngine;
using Zenject;

namespace CosmicChamps.Battle.UI
{
    public class PopupMessagePresenter : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        public class Factory : PlaceholderFactory<PopupMessagePresenter>
        {
        }

        [SerializeField]
        private TextMeshProUGUI _messageLabel;

        [SerializeField]
        private float _scaleDuration = 0.2f;

        [SerializeField]
        private float _fadeDuration = 0.5f;

        [SerializeField]
        private float _stayDuration = 2f;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        private IMemoryPool _pool;
        private Tween _tween;

        public void Display (string message)
        {
            var _transform = transform;
            _transform.localScale = Vector3.zero;
            _messageLabel.text = message;
            _messageLabel.color = _messageLabel.color.WithA (0f);

            _tween = DOTween
                .Sequence ()
                .Append (_transform.DOScale (Vector3.one, _scaleDuration).SetEase (Ease.OutElastic, 1))
                .Join (_messageLabel.DOFade (1f, _fadeDuration))
                .AppendInterval (_stayDuration)
                .Append (_messageLabel.DOFade (0f, _fadeDuration))
                .AppendCallback (Dispose);
        }

        public void OnDespawned ()
        {
            _pool = null;

            _tween?.Kill ();
            _tween = null;
        }

        public void OnSpawned (IMemoryPool pool)
        {
            _pool = pool;
        }

        public void Dispose ()
        {
            _pool?.Despawn (this);
        }
        #else
        public void OnDespawned ()
        {
        }

        public void OnSpawned (IMemoryPool pool)
        {
        }

        public void Dispose ()
        {
        }
        #endif
    }
}