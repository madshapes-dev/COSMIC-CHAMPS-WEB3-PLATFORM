using System;
using System.Collections.Generic;
using System.Linq;
using CosmicChamps.Data;
using CosmicChamps.HomeScreen.Model;
using CosmicChamps.Services;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.HomeScreen.UI
{
    public class DeckPresenter : AbstractPoolablePresenter<Unit, DeckPresenter.Callbacks, DeckPresenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action<CardPresenterModel> OnInfoClicked;

            public Callbacks (Action<CardPresenterModel> onInfoClicked)
            {
                OnInfoClicked = onInfoClicked;
            }
        }

        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private Transform _deckContainer;

        [SerializeField]
        private Transform _availableCardsContainer;

        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        private DeckSelectedCardPresenter _selectedCardPresenter;

        [SerializeField]
        private LocalizedString _noFreeSlotsError;

        [SerializeField]
        private LocalizedString _unitAlreadyInDeckError;

        [SerializeField]
        private DeckConfigurationDropdown _dropdown;

        [SerializeField]
        private ToggleGroup _switchedToggleGroup;

        [SerializeField]
        private TextMeshProUGUI _deckNameCaption;

        [SerializeField]
        private LocalizedString _deckNamePrefix;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [Inject]
        private DeckCardPresenter.Factory _cardFactory;

        [Inject]
        private DeckSwitcherToggle.Factory _deckSwitcherFactory;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private ILogger _logger;

        [Inject]
        private IMessageBroker _messageBroker;

        private readonly List<DeckCardPresenter> _deckCards = new();
        private readonly List<DeckCardPresenter> _availableCards = new();
        private Callbacks _callbacks;

        protected override void Awake ()
        {
            base.Awake ();

            _closeButton
                .OnClickAsObservable ()
                .Subscribe (_ => Hide ())
                .AddTo (this);

            _selectedCardPresenter.SetCallbacks (
                new DeckSelectedCardPresenter.Callbacks (
                    OnCardInfoClicked,
                    OnCardUseClicked,
                    OnCardRemoveClicked));

            _scrollRect
                .OnPointerDownAsObservable ()
                .Subscribe ()
                .AddTo (this);

            _scrollRect
                .OnEndDragAsObservable ()
                .Subscribe (OnScrollRectEndDrag)
                .AddTo (this);

            _scrollRect
                .OnPointerUpAsObservable ()
                .Subscribe (OnScrollRectPointerUp)
                .AddTo (this);

            _dropdown.SetCallbacks (new DeckConfigurationDropdown.Callbacks (OnDeckPresetChanged));
        }

        private void ScrollOnTopAndRefresh ()
        {
            _scrollRect
                .DOVerticalNormalizedPos (1f, 0.2f)
                .OnComplete (ForceRefresh);
        }

        private void OnDeckSelected (PlayerDeck playerDeck)
        {
            var player = _gameService.GetCachedPlayer ();
            player.ActiveDeckIndex = player.Decks.IndexOf (playerDeck);

            ScrollOnTopAndRefresh ();
        }

        private void OnScrollRectPointerUp (PointerEventData _)
        {
            _selectedCardPresenter.Hide ();
        }

        private void OnScrollRectEndDrag (PointerEventData pointerEventData)
        {
            _selectedCardPresenter.Hide ();
        }

        private void OnCardInfoClicked (CardPresenterModel cardPresenterModel)
        {
            _callbacks.OnInfoClicked?.Invoke (cardPresenterModel);
        }

        private void OnDeckPresetChanged (DeckPreset deckPreset)
        {
            var player = _gameService.GetCachedPlayer ();
            var activeDeck = player.ActiveDeck;
            activeDeck.PresetId = deckPreset.Id;
            activeDeck.Cards = deckPreset
                .Cards
                .Select (
                    x => new PlayerDeckCard
                    {
                        Id = x.Id,
                        Skin = x.Skin
                    })
                .ToArray ();

            ScrollOnTopAndRefresh ();
        }

        private void FireSoftError (LocalizedString message)
        {
            _messageBroker.Publish (
                new ErrorSignal (
                    message.GetLocalizedString (),
                    string.Empty,
                    false,
                    false,
                    false));
        }

        private void OnCardUseClicked (PlayerCard card)
        {
            var player = _gameService.GetCachedPlayer ();
            var activeDeck = player.ActiveDeck;
            var slot = activeDeck.GetFreeSlot ();
            if (!slot.HasValue)
            {
                FireSoftError (_noFreeSlotsError);
                return;
            }

            /*var gameData = _gameService.GetCachedGameData ();
            var cardUnitData = gameData.GetUnit (card.Skins[0]);
            var unitAlreadyInDeck = activeDeck
                .Cards
                .Where (x => x != null)
                .Select (x => gameData.GetUnit (x.Skin))
                .Any (x => x.DeckUniqueId == cardUnitData.DeckUniqueId);

            if (unitAlreadyInDeck)
            {
                FireSoftError (_unitAlreadyInDeckError);
                return;
            }*/
            var gameData = _gameService.GetCachedGameData ();
            var cardData = gameData.GetCard (card.Id);
            var playerUnit = player.GetUnit (cardData.UnitId);

            _selectedCardPresenter.Hide ();

            var availableCardPresenter = _availableCards.Find (x => x.model.AsPlayerCard.Item1 == card);
            availableCardPresenter.Dispose ();
            _availableCards.Remove (availableCardPresenter);

            _deckCards[slot.Value].model = activeDeck.Cards[slot.Value] = new PlayerDeckCard
            {
                Id = card.Id,
                Skin = playerUnit.Skins[0]
            };
        }

        private void OnCardRemoveClicked (CardPresenterModel cardPresenterModel)
        {
            _selectedCardPresenter.Hide ();

            var player = _gameService.GetCachedPlayer ();
            var playerCard = player.GetCard (cardPresenterModel.Id);
            var activeDeck = player.ActiveDeck;

            SpawnAvailableCard (playerCard);

            _deckCards
                .Find (x => x.model.Equals (cardPresenterModel))
                .model = (PlayerDeckCard)null;

            activeDeck.RemoveCard (cardPresenterModel.AsDeckCard);
            activeDeck.PresetId = _gameService
                .GetCachedGameData ()
                .CustomDeckPreset
                .Id;
        }

        private void OnCardClicked (DeckCardPresenter cardPresenter)
        {
            async UniTaskVoid DisplaySelectedCardPresenter ()
            {
                await _selectedCardPresenter.HideAsync (PresenterDisplayOptions.Immediate);

                _selectedCardPresenter
                    .GetRectTransform ()
                    .Align (cardPresenter.GetRectTransform (), false);

                await _selectedCardPresenter.DisplayAsync (cardPresenter.model);
                await _scrollRect.VerticalScrollForVisibility (_selectedCardPresenter.transform.GetRectTransform ());
            }

            DisplaySelectedCardPresenter ().Forget ();
        }

        private DeckCardPresenter SpawnCardPresenter (CardPresenterModel cardPresenterModel, Component parent)
        {
            var presenter = _cardFactory
                .Create ()
                .SetParent (parent)
                .AddTo (_modelDisposables);

            presenter.SetCallbacks (new DeckCardPresenter.Callbacks (OnCardClicked));
            presenter.model = cardPresenterModel;

            return presenter;
        }

        private void SpawnAvailableCard (PlayerCard card)
        {
            var gameData = _gameService.GetCachedGameData ();
            var player = _gameService.GetCachedPlayer ();
            var cardData = gameData.GetCard (card.Id);
            var playerUnit = player.GetUnit (cardData.UnitId);
            var presenter = SpawnCardPresenter ((card, playerUnit), _availableCardsContainer);
            _availableCards.Add (presenter);
        }

        private void SpawnDeckCard (PlayerDeckCard card)
        {
            var presenter = SpawnCardPresenter (card, _deckContainer);
            _deckCards.Add (presenter);
        }

        protected override void Clear ()
        {
            base.Clear ();
            _availableCards.Clear ();
            _deckCards.Clear ();
        }

        protected override void Refresh ()
        {
            ResetScrollAndSelection ();

            var player = _gameService.GetCachedPlayer ();
            var activeDeck = player.ActiveDeck;

            foreach (var slot in activeDeck.Cards)
            {
                SpawnDeckCard (slot);
            }

            var availableCards = player
                .Cards
                .Values
                .Where (x => activeDeck.Cards.All (y => x.Id != y?.Id));

            foreach (var availableCard in availableCards)
            {
                SpawnAvailableCard (availableCard);
            }

            _deckNamePrefix.Arguments = new object[]
                { new Dictionary<string, int> { { "index", player.ActiveDeckIndex + 1 } } };

            _deckNameCaption.text = _deckNamePrefix.GetLocalizedString ();
            _dropdown.model = activeDeck;

            if (_switchedToggleGroup.transform.childCount > 0)
                return;

            foreach (var deck in player.Decks)
            {
                var deckSwitcherToggle = _deckSwitcherFactory
                    .Create ()
                    .SetParent (_switchedToggleGroup)
                    .SetToggleGroup (_switchedToggleGroup)
                    .AddTo (this);

                deckSwitcherToggle.model = deck;
                deckSwitcherToggle.SetCallbacks (new DeckSwitcherToggle.Callbacks (OnDeckSelected));
            }
        }

        public override async UniTask HideAsync (PresenterDisplayOptions options = PresenterDisplayOptions.Notify)
        {
            await base.HideAsync (options);
            _scrollRect.verticalNormalizedPosition = 1f;
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);
            _callbacks = callbacks;
        }

        public void ResetScrollAndSelection ()
        {
            _selectedCardPresenter.Hide (PresenterDisplayOptions.Immediate);
            _scrollRect.verticalNormalizedPosition = 1f;
        }
        #endif
    }
}