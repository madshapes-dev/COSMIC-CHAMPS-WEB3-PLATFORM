using System;
using CosmicChamps.Battle.Data;
using CosmicChamps.Battle.Data.Client;
using CosmicChamps.Battle.UI;
using CosmicChamps.Battle.Units;
using CosmicChamps.Bootstrap.Client.UI;
using CosmicChamps.Common;
using CosmicChamps.Data;
using CosmicChamps.Level;
using CosmicChamps.Networking;
using CosmicChamps.Services;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;
using Deck = CosmicChamps.Battle.Data.Client.Deck;
using ILogger = Serilog.ILogger;
using Object = UnityEngine.Object;
using Player = CosmicChamps.Battle.Data.Client.Player;

namespace CosmicChamps.Battle.Client
{
    public class BattleService : IInitializable, IDisposable
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        private const string OvertimeMessage = "msg_overtime";

        private readonly CompositeDisposable _disposables = new();

        private readonly CameraService _cameraService;
        private readonly LevelData _levelData;
        private readonly HUDPresenter _hudPresenter;
        private readonly BattleResultPresenter _battleResultPresenter;
        private readonly CountdownPresenter _countdownPresenter;
        private readonly PopupMessagesPresenter _popupMessagesPresenter;
        private readonly ClientNetworkService _networkService;
        private readonly ITimeProvider _timeProvider;
        private readonly IGameService _gameService;
        private readonly RegularUnitNetworkBehaviour.PreviewFactory _cardPreviewFactory;
        private readonly SplashScreenPresenter _splashScreenPresenter;
        private readonly IMessageBroker _messageBroker;
        private readonly ILogger _logger;
        private readonly ICardViewDataProvider _cardViewDataProvider;

        private Player _player;
        private Opponent _opponent;
        private Data.Client.Battle _battle;
        private GameObject _unitPreview;

        public Player Player => _player;
        public Opponent Opponent => _opponent;

        public BattleService (
            CameraService cameraService,
            LevelData levelData,
            HUDPresenter hudPresenter,
            BattleResultPresenter battleResultPresenter,
            CountdownPresenter countdownPresenter,
            PopupMessagesPresenter popupMessagesPresenter,
            ClientNetworkService networkService,
            ITimeProvider timeProvider,
            IGameService gameService,
            RegularUnitNetworkBehaviour.PreviewFactory cardPreviewFactory,
            SplashScreenPresenter splashScreenPresenter,
            IMessageBroker messageBroker,
            ILogger logger,
            ICardViewDataProvider cardViewDataProvider)
        {
            _cameraService = cameraService;
            _levelData = levelData;
            _hudPresenter = hudPresenter;
            _battleResultPresenter = battleResultPresenter;
            _countdownPresenter = countdownPresenter;
            _popupMessagesPresenter = popupMessagesPresenter;
            _networkService = networkService;
            _timeProvider = timeProvider;
            _gameService = gameService;
            _cardPreviewFactory = cardPreviewFactory;
            _splashScreenPresenter = splashScreenPresenter;
            _messageBroker = messageBroker;
            _logger = logger;
            _cardViewDataProvider = cardViewDataProvider;
        }

        private void OnBattleInitialized (BattleInitData battleInitData)
        {
            async UniTaskVoid PrewarmPreviews ()
            {
                foreach (var card in _player.Deck.Cards)
                {
                    await _cardViewDataProvider.GetPreview (card.Id, card.Skin);
                }
            }

            var gameData = _gameService.GetCachedGameData ();

            _player = new Player (
                battleInitData.PlayerId,
                battleInitData.PlayerTeam,
                new Deck (battleInitData.PlayerDeck.Cards, battleInitData.PlayerDeck.NextCard),
                new PlayerEnergy (0, 0, gameData.MaxEnergy, () => Time.realtimeSinceStartup));

            _cameraService.SetActiveCamera (_player.Team);

            PrewarmPreviews ().Forget ();
        }

        private void OnNextCardReplaced (NextCardReplacement nextCardReplacement)
        {
            var deck = _player.Deck;
            deck.UpdateNextCard (nextCardReplacement.UsedCard, nextCardReplacement.NextCard);
        }

        private async UniTaskVoid OnBattleStartedAsync (BattleStartedData battleStartedData)
        {
            _battle = new Data.Client.Battle (0f, 0f, battleStartedData.MatchDuration, default);
            _opponent = new Opponent (
                battleStartedData.OpponentId,
                battleStartedData.OpponentName,
                battleStartedData.OpponentLevel,
                battleStartedData.OpponentRating);

            _hudPresenter.model = new HUDPresenter.Model (_battle, _player, _opponent);
            _hudPresenter.SetForfeitPossible (battleStartedData.ForfeitPossible);
            _hudPresenter.SetCallbacks (new HUDPresenter.Callbacks (Forfeit));

            foreach (var @base in _levelData.Bases)
            {
                @base.ShipFlyController.LandShip ();
                foreach (var spawnArea in @base.SpawnAreas)
                {
                    spawnArea.FadeOut (true);
                }
            }

            await _countdownPresenter.DisplayAsync (
                new CountdownPresenter.Model (_player, _opponent),
                PresenterDisplayOptions.Immediate);

            _countdownPresenter.StartCountdown (
                (float)(battleStartedData.CountdownDelay - _networkService.Rtt),
                battleStartedData.Countdown);

            _splashScreenPresenter.Hide ();

            await UniTask.Delay (
                TimeSpan.FromSeconds (battleStartedData.CountdownDelay + battleStartedData.Countdown - _networkService.Rtt));

            _battle.AdjustDuration (0f, _timeProvider.Time, null);
            _player
                .Energy
                .Reinitialize (battleStartedData.InitialEnergy, battleStartedData.EnergyGrowRate);

            _hudPresenter.StartTimer ();
        }

        private void OnBattleStarted (BattleStartedData battleStartedData)
        {
            OnBattleStartedAsync (battleStartedData).Forget ();
        }

        private void OnBattleOvertime (Unit _)
        {
            _popupMessagesPresenter.Display (OvertimeMessage);
        }

        private void OnBattleJoined (BattleJoinData battleJoinData)
        {
            _player
                .Energy
                .Reinitialize (battleJoinData.InitialEnergy, battleJoinData.EnergyGrowRate);

            _battle = new Data.Client.Battle (
                battleJoinData.Duration,
                (float)(_timeProvider.Time - _networkService.Rtt),
                battleJoinData.MatchDuration,
                battleJoinData.OvertimeDuration);

            _opponent = new Opponent (
                battleJoinData.OpponentId,
                battleJoinData.OpponentName,
                battleJoinData.OpponentLevel,
                battleJoinData.OpponentRating);

            _countdownPresenter.Hide ();
            _splashScreenPresenter.Hide ();
            _hudPresenter.model = new HUDPresenter.Model (_battle, _player, _opponent);
            _hudPresenter.SetForfeitPossible (battleJoinData.ForfeitPossible);
            _hudPresenter.SetCallbacks (new HUDPresenter.Callbacks (Forfeit));
            _hudPresenter.StartTimer ();

            foreach (var @base in _levelData.Bases)
            {
                @base.ShipFlyController.LandImmediate ();
                foreach (var spawnArea in @base.SpawnAreas)
                {
                    spawnArea.FadeOut (true);
                }
            }

            _player.Emoji.Value = battleJoinData.PlayerEmoji;
            _opponent.Emoji.Value = battleJoinData.OpponentEmoji;
        }

        private void OnBattleSynced (BattleSyncData battleSyncData)
        {
            _player
                .Energy
                .Reinitialize (
                    battleSyncData.EnergyValue + (float)(battleSyncData.EnergyGrowRate * _networkService.Rtt),
                    battleSyncData.EnergyGrowRate);

            _player.Emoji.Value = battleSyncData.PlayerEmoji;
            _opponent.Emoji.Value = battleSyncData.OpponentEmoji;

            _battle.AdjustDuration (
                battleSyncData.MatchDuration,
                (float)(_timeProvider.Time - _networkService.Rtt),
                battleSyncData.OvertimeDuration);
        }

        private void OnForfeitPossible (bool possible)
        {
            _hudPresenter.SetForfeitPossible (possible);
        }

        private void OnEnergyGrowRateAdjusted (EnergyGrowRate energyGrowRate)
        {
            _logger.Information ("OnEnergyGrowRateAdjusted {Message}", energyGrowRate.Message);

            _player
                .Energy
                .Reinitialize (
                    _player.Energy.Value + (float)(energyGrowRate.Rate * _networkService.Rtt),
                    energyGrowRate.Rate);

            _popupMessagesPresenter.Display (energyGrowRate.Message);
        }

        private void OnBattleFinished (BattleFinishData battleFinishData)
        {
            // _hudPresenter.StopTimer ();
            _hudPresenter.Hide ();
            _battleResultPresenter.Display (new BattleResultPresenter.Model (battleFinishData));

            Observable
                .Timer (TimeSpan.FromMilliseconds (500))
                .Subscribe (_ => _networkService.Stop ())
                .AddTo (_disposables);
        }

        public void Initialize ()
        {
            _networkService.BattlePlayerInitialized.Subscribe (OnBattleInitialized).AddTo (_disposables);
            _networkService.NextCardReplaced.Subscribe (OnNextCardReplaced).AddTo (_disposables);
            _networkService.BattleStarted.Subscribe (OnBattleStarted).AddTo (_disposables);
            _networkService.BattleOvertime.Subscribe (OnBattleOvertime).AddTo (_disposables);
            _networkService.BattleJoined.Subscribe (OnBattleJoined).AddTo (_disposables);
            _networkService.BattleSynced.Subscribe (OnBattleSynced).AddTo (_disposables);
            _networkService.ForfeitPossible.Subscribe (OnForfeitPossible).AddTo (_disposables);
            _networkService.EnergyGrowRateAdjusted.Subscribe (OnEnergyGrowRateAdjusted).AddTo (_disposables);
            _networkService.BattleFinished.Subscribe (OnBattleFinished).AddTo (_disposables);
        }

        public void Dispose ()
        {
            _disposables.Dispose ();
            _networkService.Stop ();
        }

        public bool TrySpawnUnit (string cardId, Vector2 screenPosition)
        {
            _logger.Information ("TrySpawnUnit");
            Object.Destroy (_unitPreview);

            var cardData = _gameService
                .GetCachedGameData ()
                .GetCard (cardId);

            /**
             * When the card drag began during the battle and drag finished after the battle finished, we shouldn't try to spawn the unit.
             */
            if (!_networkService.IsConnected)
                return false;

            var energy = _player.Energy;
            var requiredEnergy = cardData.Energy;
            if (energy.Value < requiredEnergy)
                return false;

            var playerCard = _player.Deck.GetCard (cardId);
            var skinId = playerCard.Skin;
            var unitData = _gameService.GetCachedGameData ().GetUnit (skinId);
            var ray = RectTransformUtility.ScreenPointToRay (_cameraService.Camera, screenPosition);

            var playerBase = _levelData.GetBase (_player.Team);
            var spawnArea = playerBase.GetSpawnArea (unitData.SpawnArea);
            if (!spawnArea.Raycast (ray, out var raycastHit, float.MaxValue))
                return false;

            var layerMask = unitData.MovementType switch
            {
                UnitMovementType.Ground => Layers.Masks.Ground,
                UnitMovementType.Spell => Layers.Masks.Ground,
                UnitMovementType.Air => Layers.Masks.Air,
                _ => throw new ArgumentOutOfRangeException ()
            };

            if (!Physics.Raycast (ray, out raycastHit, float.MaxValue, layerMask))
                return false;

            _networkService.SpawnUnit (playerCard, raycastHit.point);
            energy.Spend (requiredEnergy);

            return true;
        }

        public void FadeInSpawnArea (string spawnArea)
        {
            _levelData
                .GetBase (_player.Team)
                .GetSpawnArea (spawnArea)
                .FadeIn ();
        }

        public void FadeOutSpawnArea (string spawnArea)
        {
            _levelData
                .GetBase (_player.Team)
                .GetSpawnArea (spawnArea)
                .FadeOut ();
        }

        public void FadeInSpellArea (Vector2 screenPosition, float size)
        {
            var ray = RectTransformUtility.ScreenPointToRay (_cameraService.Camera, screenPosition);
            if (!Physics.Raycast (ray, out var raycastHit, float.MaxValue, Layers.Masks.Ground))
                return;

            var spellArea = _levelData.SpellArea;
            spellArea.FadeIn ();
            spellArea.Adjust (raycastHit.point, size * 2);
        }

        public void FadeOutSpellArea ()
        {
            _levelData.SpellArea.FadeOut ();
        }

        public async UniTaskVoid ProcessPreviewDrag (string cardId, Vector2 screenPosition)
        {
            _logger.Information ("ProcessPreviewDrag");
            var playerCard = _player.Deck.GetCard (cardId);

            if (_unitPreview == null)
            {
                _logger.Information ("_unitPreview == null, loading...");
                _unitPreview = await _cardPreviewFactory.Create (playerCard.Id, playerCard.Skin);
                _logger.Information ("_unitPreview done");
                _unitPreview
                    .transform
                    .rotation = _levelData.GetBase (_player.Team).SpawnRotation;
            }

            var skinId = playerCard.Skin;
            var unitData = _gameService.GetCachedGameData ().GetUnit (skinId);
            var layerMask = unitData.MovementType switch
            {
                UnitMovementType.Ground => Layers.Masks.Ground,
                UnitMovementType.Spell => Layers.Masks.Ground,
                UnitMovementType.Air => Layers.Masks.Air,
                _ => throw new ArgumentOutOfRangeException ()
            };

            var ray = RectTransformUtility.ScreenPointToRay (_cameraService.Camera, screenPosition);
            if (!Physics.Raycast (ray, out var raycastHit, float.MaxValue, layerMask))
                return;

            _unitPreview.SetActive (true);
            _unitPreview
                .transform
                .position = raycastHit.point;
        }

        public void HidePreview ()
        {
            if (_unitPreview != null)
                _unitPreview.SetActive (false);
        }

        public void Forfeit ()
        {
            _logger.Information ("Forfeit");

            _networkService.Forfeit ();
            _messageBroker.Publish (new RestartSignal ());
        }

        public void SetEmoji (string emoji)
        {
            //_player.Emoji.Value = emoji;
            _networkService.SetEmoji (emoji);
        }
        #else
        public void Initialize ()
        {
        }

        public void Dispose ()
        {
        }
        #endif
    }
}