using System;
using CosmicChamps.Battle.Data;
using CosmicChamps.Common;
using CosmicChamps.Services;
using CosmicChamps.UI;
using DG.Tweening;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.Battle.UI
{
    public class CardPresenter : AbstractPoolablePresenter<Card, CardPresenter.Callbacks, CardPresenter>
    {
        public class Callbacks
        {
            public readonly Action<CardPresenter> OnDragBegan;
            public readonly Action<(CardPresenter, Vector2)> OnDrag;
            public readonly Action<(CardPresenter, Vector2)> OnDragEnded;

            public Callbacks (
                Action<CardPresenter> onDragBegan,
                Action<(CardPresenter, Vector2)> onDrag,
                Action<(CardPresenter, Vector2)> onDragEnded)
            {
                OnDragBegan = onDragBegan;
                OnDrag = onDrag;
                OnDragEnded = onDragEnded;
            }
        }

        public class Factory : AbstractFactory
        {
        }

        [SerializeField]
        private ObservableEventTrigger _eventTrigger;

        [SerializeField]
        private CanvasGroup _energyCanvasGroup;

        [SerializeField]
        private float _energyFadeDuration = 0.2f;

        [SerializeField]
        private TextMeshProUGUI _energyCaption;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private float _positionRestoreDuration = 0.3f;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private float _collapseDuration = 0.2f;

        [Inject]
        private ICardViewDataProvider _cardViewDataProvider;

        [Inject]
        private IGameService _gameService;

        private RectTransform _rectTransform;
        private Vector2 _dragBeginPosition;
        private Tween _collapseTween;
        private bool _collapsed;

        protected override void Awake ()
        {
            base.Awake ();
            _rectTransform = this.GetRectTransform ();
        }

        private void PerformDrag (PointerEventData pointerEventData)
        {
            _rectTransform.anchoredPosition += pointerEventData.ScreenDeltaToLocalPointInRectangle (_rectTransform);
        }

        private async void LoadCardSprite ()
        {
            var cardSprite = await _cardViewDataProvider.GetCardSprite (model.Id, model.Skin);
            _icon.sprite = cardSprite;
        }

        private void KillCollapseTween ()
        {
            _collapseTween?.Kill ();
            _collapseTween = null;
        }

        protected override void Refresh ()
        {
            Expand (true);
            LoadCardSprite ();

            var cardData = _gameService.GetCachedGameData ().GetCard (model.Id);
            _energyCaption.text = cardData.Energy.ToString ();
        }

        protected override void Clear ()
        {
            base.Clear ();
            KillCollapseTween ();
            _callbacksDisposables.Clear ();
        }

        public void RestorePositionAfterDrag ()
        {
            _rectTransform.DOAnchorPos (_dragBeginPosition, _positionRestoreDuration);
            if (_collapsed)
                Expand ();
        }

        public void Collapse (bool immediate = false)
        {
            if (_collapsed)
                return;

            KillCollapseTween ();
            if (immediate)
                _canvasGroup.alpha = 0f;
            else
                _collapseTween = _canvasGroup.DOFade (0f, _collapseDuration);

            _collapsed = true;
        }

        public void Expand (bool immediate = false)
        {
            if (!_collapsed)
                return;

            KillCollapseTween ();
            if (immediate)
                _canvasGroup.alpha = 1f;
            else
                _collapseTween = _canvasGroup.DOFade (1f, _collapseDuration);

            _collapsed = false;
        }

        public void FadeInEnergy (bool immediate = false)
        {
            if (immediate)
                _energyCanvasGroup.alpha = 1f;
            else
                _energyCanvasGroup.DOFade (1f, _energyFadeDuration);
        }

        public void FadeOutEnergy (bool immediate = false)
        {
            if (immediate)
                _energyCanvasGroup.alpha = 0f;
            else
                _energyCanvasGroup.DOFade (0f, _energyFadeDuration);
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            if (callbacks == null)
                return;

            void OnDragBegin (PointerEventData pointerEventData)
            {
                _dragBeginPosition = _rectTransform.anchoredPosition;
                PerformDrag (pointerEventData);

                callbacks
                    .OnDragBegan
                    ?.Invoke (this);
            }

            void OnDrag (PointerEventData pointerEventData)
            {
                PerformDrag (pointerEventData);
                callbacks.OnDrag?.Invoke ((this, pointerEventData.position));
            }

            _eventTrigger
                .OnBeginDragAsObservable ()
                .Subscribe (OnDragBegin)
                .AddTo (_callbacksDisposables);

            _eventTrigger
                .OnDragAsObservable ()
                .Subscribe (OnDrag)
                .AddTo (_callbacksDisposables);

            _eventTrigger
                .OnEndDragAsObservable ()
                .Subscribe (x => callbacks.OnDragEnded?.Invoke ((this, x.position)))
                .AddTo (_callbacksDisposables);
        }
    }
}