using System;
using CosmicChamps.Common;
using CosmicChamps.Data;
using CosmicChamps.HomeScreen.Model;
using CosmicChamps.Services;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class DeckSelectedCardPresenter : AbstractPresenter<
        CardPresenterModel,
        DeckSelectedCardPresenter.Callbacks,
        DeckSelectedCardPresenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action<CardPresenterModel> OnInfoClicked;
            public readonly Action<PlayerCard> OnUseClicked;
            public readonly Action<CardPresenterModel> OnRemoveClicked;

            public Callbacks (
                Action<CardPresenterModel> onInfoClicked,
                Action<PlayerCard> onUseClicked,
                Action<CardPresenterModel> onRemoveClicked)
            {
                OnInfoClicked = onInfoClicked;
                OnUseClicked = onUseClicked;
                OnRemoveClicked = onRemoveClicked;
            }
        }

        [SerializeField]
        private Image _avatar;

        [SerializeField]
        private TextMeshProUGUI _nameCaption;

        [SerializeField]
        private Button _infoButton;

        [SerializeField]
        private Button _useButton;

        [SerializeField]
        private Button _removeButton;

        [SerializeField]
        private TextMeshProUGUI _energyCaption;

        [SerializeField]
        private TextMeshProUGUI _levelCaption;

        [SerializeField]
        private RectTransform _levelUpPossible;

        [SerializeField]
        private LocalizedString _infoString;

        [SerializeField]
        private LocalizedString _upgradeString;

        [SerializeField]
        private TextMeshProUGUI _infoButtonCaption;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [Inject]
        private ICardViewDataProvider _cardViewDataProvider;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private Captions _captions;

        private Canvas _canvas;
        private GraphicRaycaster _graphicRaycaster;

        private async UniTaskVoid LoadAvatar ()
        {
            _avatar.sprite = await _cardViewDataProvider.GetCardSprite (model.Id, model.Skin);
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _infoButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnInfoClicked?.Invoke (model))
                .AddTo (_callbacksDisposables);

            _useButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnUseClicked?.Invoke (model.AsT1.Item1))
                .AddTo (_callbacksDisposables);

            _removeButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnRemoveClicked?.Invoke (model.AsT0))
                .AddTo (_callbacksDisposables);
        }

        protected override void Refresh ()
        {
            var gameData = _gameService.GetCachedGameData ();
            var player = _gameService.GetCachedPlayer ();
            var cardData = gameData.GetCard (model.Id);
            var playerCard = player.GetCard (model.Id);

            _nameCaption.text = cardData.DisplayName;
            _energyCaption.text = cardData.Energy.ToString ();
            playerCard
                .Level
                .Select (_captions.CardLevel)
                .Subscribe (
                    x =>
                    {
                        _levelCaption.text = x;

                        var levelUpPossible = playerCard.SimpleLevelUpPossible (player, gameData);
                        _levelUpPossible.SetVisible (levelUpPossible);
                        _infoButtonCaption.text =
                            (levelUpPossible ? _upgradeString : _infoString).GetLocalizedString ();
                    })
                .AddTo (_modelDisposables);

            _useButton.SetVisible (model.IsT1);
            _removeButton.SetVisible (model.IsT0);

            model.Switch (
                slot =>
                {
                    slot
                        .ObserveEveryValueChanged (x => x.Skin)
                        .Subscribe (_ => LoadAvatar ().Forget ())
                        .AddTo (_modelDisposables);
                },
                _ => LoadAvatar ().Forget ());
        }
        #endif
    }
}