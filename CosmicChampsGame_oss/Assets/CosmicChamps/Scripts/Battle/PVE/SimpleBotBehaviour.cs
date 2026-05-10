using System;
using System.Collections.Generic;
using System.Linq;
using CosmicChamps.Battle.Data;
using CosmicChamps.Data;
using CosmicChamps.Level;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using UniRx;
using UnityEngine;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Battle.PVE
{
    #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
    public class SimpleBotBehaviour : IInitializable
    {
        private const int HeavyUnitEnergyThreshold = 5;
        private const int HeavyUnitUsePeriod = 4;

        private readonly CompositeDisposable _disposables = new();
        private readonly List<Card> _cardsToUseSet = new();

        private readonly IBotNetworkService _networkService;
        private readonly GameDataRepository _gameDataRepository;
        private readonly LevelData _levelData;
        private readonly ILogger _logger;

        private Deck _deck;
        private float _energyValue;
        private PlayerTeam _team;
        private Card _nextCardToUse;
        private int _cardUseCounter;

        public SimpleBotBehaviour (
            IBotNetworkService networkService,
            GameDataRepository gameDataRepository,
            LevelData levelData,
            ILogger logger)
        {
            _networkService = networkService;
            _gameDataRepository = gameDataRepository;
            _levelData = levelData;
            _logger = logger;
        }

        private void OnBattleStating (Unit _)
        {
            _logger.Information ("OnBattleStating");
            _networkService.LevelLoaded ();
        }

        private void OnBattlePlayerInitialized (BattleInitData battleInitData)
        {
            _logger.Information ("OnBattlePlayerInitialized");
            _deck = battleInitData.PlayerDeck;
            _team = battleInitData.PlayerTeam;
            _cardUseCounter = 0;
            PickNextCardToUse ();
        }

        private bool IsLightUnitCard (Card card) =>
            _gameDataRepository.GetCachedGameData ().Cards[card.Id].Energy < HeavyUnitEnergyThreshold;

        private bool IsHeavyUnitCard (Card card) =>
            _gameDataRepository.GetCachedGameData ().Cards[card.Id].Energy >= HeavyUnitEnergyThreshold;

        private bool PickUnitCardToUse (Predicate<Card> predicate, out Card card)
        {
            _cardsToUseSet.Clear ();
            foreach (var deckCard in _deck.Cards)
            {
                if (predicate (deckCard))
                    _cardsToUseSet.Add (deckCard);
            }

            var result = _cardsToUseSet.GetRandom (out card);
            _cardsToUseSet.Clear ();
            return result;
        }

        private void PickNextCardToUse ()
        {
            (Predicate<Card> predicate, string name) primaryPredicate = (IsLightUnitCard, nameof (IsLightUnitCard));
            (Predicate<Card> predicate, string name) secondaryPredicate = (IsHeavyUnitCard, nameof (IsHeavyUnitCard));

            if (_cardUseCounter % HeavyUnitUsePeriod == 0)
            {
                primaryPredicate = (IsHeavyUnitCard, nameof (IsHeavyUnitCard));
                secondaryPredicate = (IsLightUnitCard, nameof (IsLightUnitCard));
            }

            if (PickUnitCardToUse (primaryPredicate.predicate, out _nextCardToUse))
            {
                _logger.Information (
                    "PickNextCardToUse _cardUseCounter {CardUseCounter}, {PrimaryPredicateName} used",
                    _cardUseCounter,
                    primaryPredicate.name);
                return;
            }

            if (PickUnitCardToUse (secondaryPredicate.predicate, out _nextCardToUse))
            {
                _logger.Information (
                    "PickNextCardToUse _cardUseCounter {CardUseCounter}, {SecondaryPredicateName} used",
                    _cardUseCounter,
                    secondaryPredicate.name);
                return;
            }

            throw new InvalidOperationException (
                $"Unable to pick next card to use, cards: {string.Join (", ", _deck.Cards.Select (x => x.Id))}; _cardUseCounter {_cardUseCounter}");
        }

        private void OnBattleSynced (BattleSyncData battleSyncData)
        {
            _energyValue = battleSyncData.EnergyValue;

            if (_nextCardToUse == null)
                return;

            var gameData = _gameDataRepository.GetCachedGameData ();
            if (gameData.Cards[_nextCardToUse.Id].Energy > _energyValue)
                return;

            _logger.Information (
                "OnBattleSynced spawn unit _cardUseCounter {CardUseCounter}, _nextCardToUse {Id}",
                _cardUseCounter,
                _nextCardToUse.Id);

            var unitData = gameData.GetUnit (_nextCardToUse.Skin);
            var layerMask = unitData.MovementType switch
            {
                UnitMovementType.Ground => Layers.Masks.Ground,
                UnitMovementType.Air => Layers.Masks.Air,
                _ => throw new ArgumentOutOfRangeException ()
            };
            var spawnAreaRandomPosition = _levelData
                .GetBase (_team)
                .GetSpawnArea (unitData.SpawnArea)
                .GetRandomPosition ();

            var ray = new Ray (spawnAreaRandomPosition.WithY (1000f), Vector3.down);
            if (!Physics.Raycast (ray, out var raycastHit, float.MaxValue, layerMask))
                throw new InvalidOperationException ($"Raycast failed, ray: {ray}");

            _networkService.SpawnUnit (_nextCardToUse, raycastHit.point);
            _nextCardToUse = null;
        }

        private void OnNextCardReplaced (NextCardReplacement nextCardReplacement)
        {
            _logger.Information (
                "OnNextCardReplaced UsedCard: {UsedCardId}; NextCard: {NextCardId}",
                nextCardReplacement.UsedCard.Id,
                nextCardReplacement.NextCard.Id);

            var usedCard = nextCardReplacement.UsedCard;
            var nextCard = nextCardReplacement.NextCard;

            var usedCardIndex = Array.IndexOf (_deck.Cards, usedCard);
            if (usedCardIndex < 0)
                throw new InvalidOperationException ($"Card {usedCard.Id} not found in the deck");

            _deck.Cards[usedCardIndex] = _deck.NextCard;
            _deck.NextCard = nextCard;

            _cardUseCounter++;
            PickNextCardToUse ();
        }

        private void OnBattleFinished (BattleFinishData battleFinishData)
        {
            _logger.Information (
                "OnBattleFinished result: {Result}; message: {Message}",
                battleFinishData.Result,
                battleFinishData.Message);

            Shutdown ();
        }

        public void Initialize ()
        {
            _logger.Information ("Initialize");

            _networkService.BattleStarting.Subscribe (OnBattleStating).AddTo (_disposables);
            _networkService.BattleSynced.Subscribe (OnBattleSynced).AddTo (_disposables);
            _networkService.NextCardReplaced.Subscribe (OnNextCardReplaced).AddTo (_disposables);
            _networkService.BattlePlayerInitialized.Subscribe (OnBattlePlayerInitialized).AddTo (_disposables);
            _networkService.BattleFinished.Subscribe (OnBattleFinished).AddTo (_disposables);
        }

        public void Shutdown ()
        {
            _logger.Information ("Shutdown");

            _disposables.Dispose ();

            async UniTaskVoid Disconnect ()
            {
                _logger.Information ("Disconnect delay...");
                await UniTask.Delay (TimeSpan.FromMilliseconds (500));
                _logger.Information ("Disconnect");
                _networkService.Stop ();
            }

            Disconnect ().Forget ();
        }
    }
    #endif
}