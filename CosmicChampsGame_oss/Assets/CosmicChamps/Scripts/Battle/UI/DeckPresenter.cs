using System;
using System.Collections.Generic;
using CosmicChamps.Battle.Client;
using CosmicChamps.Battle.Data;
using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ThirdParty.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using Deck = CosmicChamps.Battle.Data.Client.Deck;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Battle.UI
{
    public class DeckPresenter : AbstractPresenter<DeckPresenter.Model, DeckPresenter.Callbacks, DeckPresenter>
    {
        public readonly struct Model
        {
            public readonly Deck Deck;

            public Model (Deck deck)
            {
                Deck = deck;
            }
        }

        public readonly struct Callbacks
        {
        }

        [SerializeField]
        private RectTransform _nextCardPlaceholder;

        [SerializeField]
        private RectTransform[] _cardPlaceholders;

        [SerializeField]
        private float _cardHandOutDuration = 0.2f;

        [SerializeField]
        private float _cardHandOutPeriod = 0.2f;

        [SerializeField]
        private Transform _cardsContainer;

        [SerializeField]
        private RectTransform _cardExpandArea;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [Inject]
        private CardPresenter.Factory _cardFactory;

        [Inject]
        private BattleService _battleService;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private ILogger _logger;

        private Tween _handOutTween;
        private Tween _nextCardTween;
        private Canvas _rootCanvas;
        private CardPresenter _nextCardPresenter;
        private List<CardPresenter> _cardPresenters;
        private GameObject _unitPreview;

        private void OnCardReplace (CollectionReplaceEvent<Card> cardReplaceEvent)
        {
            _cardPresenters[cardReplaceEvent.Index].Dispose ();

            _nextCardPresenter.SetCallbacks (new CardPresenter.Callbacks (OnCardDragBegan, OnCardDrag, OnCardDragEnded));
            _cardPresenters[cardReplaceEvent.Index] = _nextCardPresenter;

            var cardTransform = _nextCardPresenter.GetRectTransform ();
            var cardPlaceholder = _cardPlaceholders[cardReplaceEvent.Index];

            _nextCardPresenter.FadeInEnergy ();
            cardTransform.SetAsLastSibling ();

            _nextCardTween = DOTween
                .Sequence ()
                .Append (cardTransform.DOMove (cardPlaceholder.position, _cardHandOutDuration))
                .Join (cardTransform.DOSizeDelta (cardPlaceholder.sizeDelta, _cardHandOutDuration));
        }

        private void SpawnNextCard (Card card)
        {
            _nextCardPresenter = SpawnCardOnTop ();
            _nextCardPresenter.model = card;
            _nextCardPresenter.SetSiblingIndex (0);
        }

        private void OnNextCard (Card card)
        {
            SpawnNextCard (card);
        }

        private CardPresenter SpawnCardOnTop ()
        {
            var cardPresenter = _cardFactory
                .Create ()
                .AddTo (_modelDisposables);

            cardPresenter.FadeOutEnergy (true);

            var cardTransform = cardPresenter.GetRectTransform ();
            cardTransform.SetParent (_cardsContainer);
            cardPresenter.SetAsLastSibling ();
            cardTransform.position = _nextCardPlaceholder.position;
            cardTransform.sizeDelta = _nextCardPlaceholder.sizeDelta;
            cardTransform.localScale = Vector3.one;

            return cardPresenter;
        }

        private void OnCardDragBegan (CardPresenter cardPresenter)
        {
            _logger.Information ("OnCardDragBegan");

            var unitData = _gameService
                .GetCachedGameData ()
                .GetUnit (cardPresenter.model.Skin);

            _battleService.FadeInSpawnArea (unitData.SpawnArea);
        }

        private void OnCardDrag ((CardPresenter, Vector2) _)
        {
            _logger.Information ("OnCardDrag");

            var (cardPresenter, dragPosition) = _;
            var unitData = _gameService
                .GetCachedGameData ()
                .GetUnit (cardPresenter.model.Skin);
            var shouldExpand = RectTransformUtility.RectangleContainsScreenPoint (_cardExpandArea, dragPosition);
            if (shouldExpand)
            {
                cardPresenter.Expand ();

                switch (unitData.SpawnArea)
                {
                    case UnitSpawnArea.Spell:
                        _battleService.FadeOutSpellArea ();
                        break;
                    default:
                        _battleService.HidePreview ();
                        break;
                }
            } else
            {
                cardPresenter.Collapse ();

                switch (unitData.SpawnArea)
                {
                    case UnitSpawnArea.Spell:
                        _battleService.FadeInSpellArea (dragPosition, unitData.Stats.Damage.Range);
                        break;
                    default:
                        _battleService.ProcessPreviewDrag (cardPresenter.model.Id, dragPosition).Forget ();
                        break;
                }
            }
        }

        private void OnCardDragEnded ((CardPresenter, Vector2) _)
        {
            _logger.Information ("OnCardDragEnded");

            var (cardPresenter, dragEndPosition) = _;
            var unitData = _gameService
                .GetCachedGameData ()
                .GetUnit (cardPresenter.model.Skin);

            var spawnSucceed = _battleService.TrySpawnUnit (cardPresenter.model.Id, dragEndPosition);

            if (unitData.SpawnArea == UnitSpawnArea.Spell)
            {
                async UniTaskVoid FadeOutSpellArea ()
                {
                    if (spawnSucceed)
                        await UniTask.Delay (TimeSpan.FromSeconds (unitData.ViewParams.DamageDelay));

                    _battleService.FadeOutSpellArea ();
                }

                FadeOutSpellArea ().Forget ();
            }

            _battleService.FadeOutSpawnArea (unitData.SpawnArea);

            if (!spawnSucceed)
                cardPresenter.RestorePositionAfterDrag ();
        }

        protected override void Refresh ()
        {
            var deck = model.Deck;
            var cards = deck.Cards;

            cards
                .ObserveReplace ()
                .Subscribe (OnCardReplace)
                .AddTo (_modelDisposables);

            deck
                .NextCard
                .Subscribe (OnNextCard)
                .AddTo (_modelDisposables);

            _cardPresenters = UnityEngine.Pool.ListPool<CardPresenter>.Get ();
            for (var i = cards.Count - 1; i >= 0; i--)
            {
                var cardPresenter = SpawnCardOnTop ();
                cardPresenter.model = cards[i];
                cardPresenter.SetCallbacks (new CardPresenter.Callbacks (OnCardDragBegan, OnCardDrag, OnCardDragEnded));
                _cardPresenters.Add (cardPresenter);
            }

            _cardPresenters.Reverse ();

            var sequence = DOTween.Sequence ();
            for (var i = 0; i < _cardPresenters.Count; i++)
            {
                var cardPresenter = _cardPresenters[i];
                var cardTransform = cardPresenter.GetRectTransform ();
                if (i >= _cardPlaceholders.Length)
                    throw new InvalidOperationException ("Card placeholders and cards count mismatch");

                var cardPlaceholder = _cardPlaceholders[i];

                sequence
                    .AppendCallback (() => cardTransform.SetAsLastSibling ())
                    .AppendCallback (() => cardPresenter.FadeInEnergy ())
                    .Append (cardTransform.DOMove (cardPlaceholder.position, _cardHandOutDuration))
                    .Join (cardTransform.DOSizeDelta (cardPlaceholder.sizeDelta, _cardHandOutDuration))
                    .AppendInterval (_cardHandOutPeriod);
            }

            _handOutTween = sequence;
        }

        protected override void Clear ()
        {
            base.Clear ();

            if (_cardPresenters != null)
            {
                UnityEngine.Pool.ListPool<CardPresenter>.Release (_cardPresenters);
                _cardPresenters = null;
            }

            _handOutTween?.Kill ();
            _handOutTween = null;

            _nextCardTween?.Kill ();
            _nextCardTween = null;
        }
        #endif
    }
}