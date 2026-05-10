using System;
using System.Linq;
using CosmicChamps.Battle.Data;
using CosmicChamps.UI;
using DG.Tweening;
using ThirdParty.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicChamps.Battle
{
    public class BaseUnitHPBar : MonoBehaviour
    {
        [Serializable]
        private class Skin
        {
            [SerializeField]
            private PlayerTeam _playerTeam;

            [SerializeField]
            private Sprite _bar;

            [SerializeField]
            private Sprite _icon;

            public PlayerTeam PlayerTeam => _playerTeam;

            public void Apply (Image icon, Image bar)
            {
                icon.sprite = _icon;
                bar.sprite = _bar;
            }
        }

        [SerializeField]
        private TextMeshProUGUI _caption;

        [SerializeField]
        private float _captionFadeDuration = 0.2f;

        [SerializeField]
        private Progressbar _progressbar;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private Image _bar;

        [SerializeField]
        private Skin[] _skins;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private float _fadeDuration;

        private Tween _fadeTween;
        private RectTransform _rectTransform;

        private void Awake ()
        {
            _rectTransform = transform as RectTransform;
        }

        public void SetPlayerTeam (PlayerTeam playerTeam)
        {
            var skin = _skins.FirstOrDefault (x => x.PlayerTeam == playerTeam);
            if (skin == null)
                throw new InvalidOperationException ($"Cannot find skin for team {playerTeam}");

            skin.Apply (_icon, _bar);
        }

        public void SetValue (UnitHp hp, bool immediate = false)
        {
            if (immediate)
                _caption.text = hp.Value.ToString ();
            else
                _caption.AnimateTextChangeThroughFade (hp.Value.ToString (), _captionFadeDuration);

            _progressbar.SetValue (hp.NormalizedValue);
        }

        public void FadeIn (bool immediate = false, float delay = 0f)
        {
            _fadeTween?.Kill ();
            _fadeTween = null;

            if (immediate)
                _canvasGroup.alpha = 1f;
            else
                _fadeTween = _canvasGroup.DOFade (1f, _fadeDuration).SetDelay (delay);
        }

        public void FadeOut (bool immediate = false, float delay = 0f)
        {
            _fadeTween?.Kill ();
            _fadeTween = null;

            if (immediate)
                _canvasGroup.alpha = 0f;
            else
                _fadeTween = _canvasGroup.DOFade (0f, _fadeDuration).SetDelay (delay);
        }

        public void Align (Vector3 worldPosition) => _rectTransform.AlignToWorldPosition (worldPosition);
    }
}