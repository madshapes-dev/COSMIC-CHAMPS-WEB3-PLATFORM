using System;
using CosmicChamps.Common;
using CosmicChamps.Data;
using CosmicChamps.HomeScreen.Model;
using CosmicChamps.Services;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using ThirdParty.Extensions.CanvasGroupFader;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.HomeScreen.UI
{
    public class DeckCardPresenter : AbstractPoolablePresenter<
        CardPresenterModel,
        DeckCardPresenter.Callbacks,
        DeckCardPresenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action<DeckCardPresenter> OnClicked;

            public Callbacks (Action<DeckCardPresenter> onClicked)
            {
                OnClicked = onClicked;
            }
        }

        public class Factory : AbstractFactory
        {
        }

        [SerializeField]
        private Image _avatar;

        [SerializeField]
        private TextMeshProUGUI _nameCaption;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private RectTransform _empty;

        [SerializeField]
        private RectTransform _card;

        [SerializeField]
        private TextMeshProUGUI _energyCaption;

        [SerializeField]
        private TextMeshProUGUI _levelCaption;

        [SerializeField]
        private RectTransform _levelUpPossible;

        [SerializeField]
        private RectTransform _levelLockBlock;

        [SerializeField]
        private TextMeshProUGUI _levelLockCaption;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [Inject]
        private ICardViewDataProvider _cardViewDataProvider;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private Captions _captions;

        [Inject]
        private ILogger _logger;

        private readonly CompositeDisposable _cardDisposables = new();

        private Canvas _canvas;
        private GraphicRaycaster _graphicRaycaster;

        private async UniTaskVoid LoadAvatarAsync ()
        {
            // _logger.Information ("LoadAvatarAsync {HashCode} id {Id} skin {Skin}...", GetHashCode (), model.Id, model.Skin);
            var sprite = await _cardViewDataProvider.GetCardSprite (model.Id, model.Skin);
            /*_logger.Information (
                "LoadAvatarAsync {HashCode} id {Id} skin {Skin} sprite {Sprite} done",
                GetHashCode (),
                model.Id,
                model.Skin,
                sprite.GetHashCode ());*/

            _avatar.sprite = sprite;
        }

        private void LoadAvatar ()
        {
            // _logger.Information ("LoadAvatar");
            LoadAvatarAsync ().Forget ();
        }

        private void EmptySlotRefresh (bool immediate)
        {
            _empty.FadeIn (immediate);
            _card.FadeOut (immediate);
        }

        private void CardRefresh (bool immediate)
        {
            _cardDisposables.Clear ();

            var gameData = _gameService.GetCachedGameData ();
            var player = _gameService.GetCachedPlayer ();
            var cardData = gameData.GetCard (model.Id);
            var playerCard = player.GetCard (model.Id);
            var playerUnit = player.GetUnit (cardData.UnitId);

            _empty.FadeOut (immediate);
            _card.FadeIn (immediate);

            _nameCaption.text = cardData.DisplayName;
            _energyCaption.text = cardData.Energy.ToString ();
            _levelLockCaption.text = _captions.CardUnlockAt (cardData.LevelLock);

            playerCard
                .Level
                .Select (_captions.CardLevel)
                .Subscribe (
                    x =>
                    {
                        _levelCaption.text = x;
                        _levelUpPossible.SetVisible (playerCard.SimpleLevelUpPossible (player, gameData));
                    })
                .AddTo (_cardDisposables);

            player
                .Level
                .Subscribe (
                    _ => _levelLockBlock.SetVisible (
                        playerUnit.Skins.Length == 1 && cardData.LevelLock > player.Level.Value))
                .AddTo (_cardDisposables);

            LoadAvatar ();
        }

        private void SlotRefresh (PlayerDeckCard card, bool immediate)
        {
            if (card == null)
                EmptySlotRefresh (immediate);
            else
                CardRefresh (immediate);
        }

        protected override void Clear ()
        {
            base.Clear ();

            _empty.FadeOut (true);
            _card.FadeOut (true);

            _cardDisposables.Clear ();
        }

        protected override void Refresh ()
        {
            model.Switch (
                deckCard =>
                {
                    SlotRefresh (deckCard, true);

                    deckCard
                        ?.ObserveEveryValueChanged (x => x.Skin)
                        .Skip (1)
                        .Subscribe (_ => LoadAvatar ())
                        .AddTo (_modelDisposables);
                },
                _ => CardRefresh (true));
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _button
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnClicked?.Invoke (this))
                .AddTo (_callbacksDisposables);
        }
        #endif
    }
}