using System;
using System.Collections.Generic;
using System.Linq;
using CosmicChamps.Battle.Data;
using CosmicChamps.Battle.Units;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using CosmicChamps.Bootstrap.Server;
using CosmicChamps.Data;
using CosmicChamps.Level;
using CosmicChamps.Networking;
using CosmicChamps.Services;
using Cysharp.Threading.Tasks;
using Pathfinding;
using UniRx;
using UnityEngine;
using Zenject;
using Deck = CosmicChamps.Battle.Data.Server.Deck;
using ILogger = Serilog.ILogger;
using Player = CosmicChamps.Battle.Data.Server.Player;
using BattleResult = CosmicChamps.Data.BattleResult;

namespace CosmicChamps.Battle.Server
{
    public class BattleService : IInitializable, IDisposable, ITargetsProvider
    {
        public readonly struct Args
        {
            public readonly float BattleSyncInterval;

            public Args (float battleSyncInterval)
            {
                BattleSyncInterval = battleSyncInterval;
            }
        }

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private readonly struct Unit
        {
            public readonly IUnit Self;
            public readonly ITarget Target;

            public Unit (IUnit self)
            {
                Self = self;
                Target = self.GetUnitComponent<ITarget> ();
            }

            public bool Equals (Unit other)
            {
                return Equals (Self, other.Self);
            }

            public override bool Equals (object obj)
            {
                return obj is Unit other && Equals (other);
            }

            public override int GetHashCode ()
            {
                return Self != null ? Self.GetHashCode () : 0;
            }

            public static Unit FromIUnit (IUnit unit) => new(unit);
        }

        private const string OpponentLeftMessage = "msg_opponent_left";
        private const string OpponentDefeatedMessage = "msg_opponent_defeated";
        private const string OpponentForfeitedMessage = "msg_oppoent_forfeited";
        private const string YouDefeatedMessage = "msg_you_defeated";
        private const string DrawGameMessage = "msg_drawn_game";

        private readonly CompositeDisposable _disposables = new();
        private readonly CompositeDisposable _battleDisposables = new();
        private readonly Dictionary<AuthData, Player> _players = new();
        private readonly Dictionary<PlayerTeam, List<Unit>> _units = new();
        private readonly RegularUnitNetworkBehaviour.ServerFactory _unitFactory;
        private readonly BaseUnitNetworkBehaviour.ServerFactory _baseUnitFactory;
        private readonly LevelData _levelData;
        private readonly IGameService _gameService;
        private readonly Args _args;
        private readonly ServerNetworkService _networkService;
        private readonly ITimeProvider _timeProvider;
        private readonly GameSessionProvider _gameSessionProvider;
        private readonly ILogger _logger;

        private IDisposable _reconnectionTimer;
        private bool _battleStarted;
        private bool _overtime;
        private float _battleStartTime;

        public BattleService (
            RegularUnitNetworkBehaviour.ServerFactory unitFactory,
            BaseUnitNetworkBehaviour.ServerFactory baseUnitFactory,
            LevelData levelData,
            IGameService gameService,
            Args args,
            ServerNetworkService networkService,
            ITimeProvider timeProvider,
            GameSessionProvider gameSessionProvider,
            ILogger logger)
        {
            _unitFactory = unitFactory;
            _baseUnitFactory = baseUnitFactory;
            _levelData = levelData;
            _gameService = gameService;
            _args = args;
            _networkService = networkService;
            _timeProvider = timeProvider;
            _gameSessionProvider = gameSessionProvider;
            _logger = logger;
        }

        private float GetEnergyGrowRate ()
        {
            var battleTime = _timeProvider.Time - _battleStartTime;
            var gameData = _gameService.GetCachedGameData ();
            var energyGrowRate = gameData.EnergyGrowRates.FirstOrDefault (x => x.From >= battleTime) ??
                                 gameData.EnergyGrowRates.Last ();

            return energyGrowRate.Rate;
        }

        private float GetDuration () => _timeProvider.Time - _battleStartTime;

        private void AddUnits (PlayerTeam playerTeam, params IUnit[] units)
        {
            if (!_units.TryGetValue (playerTeam, out var playerUnits))
            {
                playerUnits = new List<Unit> ();
                _units.Add (playerTeam, playerUnits);
            }

            playerUnits.AddRange (units.Select (Unit.FromIUnit));

            units
                .Select (x => x.OnDying.Select (_ => x))
                .Merge ()
                .Subscribe (
                    x =>
                    {
                        if (_units.TryGetValue (x.Team, out var teamUnits))
                            teamUnits.Remove (Unit.FromIUnit (x));
                    })
                .AddTo (_disposables);
        }

        private void SpawnBaseUnits (Player player)
        {
            var @base = _levelData.GetBase (player.Team);
            var gameData = _gameService.GetCachedGameData ();
            var shipCardData = gameData.GetShipCard (player.ShipCard);
            var baseUnitData = gameData.GetBaseUnit (shipCardData.MainId);
            var turretUnitData = gameData.GetBaseUnit (shipCardData.TurretId);
            var shieldUnitData = gameData.GetBaseUnit (shipCardData.ShieldId);

            var baseUnit = _baseUnitFactory.Create (
                player.Id,
                @base.BasePlaceholder,
                player.Team,
                BaseUnitType.Base,
                baseUnitData,
                player.Level);

            var turretUnit = _baseUnitFactory.Create (
                player.Id,
                @base.TurretPlaceholder,
                player.Team,
                BaseUnitType.Turret,
                turretUnitData,
                0);

            var shieldUnit = _baseUnitFactory.Create (
                player.Id,
                @base.ShieldPlaceholder,
                player.Team,
                BaseUnitType.Shield,
                shieldUnitData,
                0);

            AddUnits (player.Team, baseUnit, turretUnit, shieldUnit);

            _networkService.SpawnBaseUnit (baseUnit);
            _networkService.SpawnBaseUnit (turretUnit);
            _networkService.SpawnBaseUnit (shieldUnit);

            baseUnit
                .OnDying
                .Subscribe (OnBaseDying)
                .AddTo (_battleDisposables);

            turretUnit
                .OnDying
                .Merge (shieldUnit.OnDying)
                .Where (_ => _overtime)
                .Subscribe (OnTurretDying)
                .AddTo (_battleDisposables);
        }

        private void OnBaseDying (IUnit unit)
        {
            _logger.Information ("OnBaseDying {Team}", unit.Team);
            ReportBattleResult (unit.Team.GetOpposite ());
        }

        private void OnTurretDying (IUnit unit)
        {
            _logger.Information ("OnTurretDying {Team}", unit.Team);
            ReportBattleResult (unit.Team.GetOpposite ());
        }

        private Player GetPlayer (AuthData authData, bool suppressException = false)
        {
            if (!_players.TryGetValue (authData, out var player) && !suppressException)
                throw new InvalidOperationException ($"No battle player found for {authData}");

            return player;
        }

        private void OnClientLevelLoaded (AuthData authData)
        {
            _logger.Information (
                "OnClientLevelLoaded id: {Id} sessionId: {SessionId}",
                authData.PlayerId,
                authData.PlayerSessionId);

            var gameData = _gameService.GetCachedGameData ();

            _logger.Information ("OnClientLevelLoaded _players.Count {PlayersCount}", _players.Count);

            Player battlePlayer;
            if (_players.Count == 2)
            {
                _logger.Information ("OnClientLevelLoaded setting online for the player");
                battlePlayer = GetPlayer (authData);
                battlePlayer.Online = true;
            } else
            {
                _logger.Information ("OnClientLevelLoaded adding new player");

                var player = _gameSessionProvider
                    .GameSession
                    .GetPlayer (authData.PlayerId);
                var playerDeck = player.ActiveDeck;
                var cards = playerDeck.Cards.Select (
                    x => new Card
                    {
                        Id = x.Id,
                        Skin = x.Skin
                    });

                battlePlayer = new Player (
                    player,
                    _players.Count == 0 ? PlayerTeam.North : PlayerTeam.South,
                    new Deck (cards, gameData.OpenedCardsCount),
                    new PlayerEnergy (
                        gameData.InitialEnergy,
                        _battleStarted ? gameData.EnergyGrowRates[0].Rate : 0,
                        gameData.MaxEnergy,
                        () => Time.realtimeSinceStartup),
                    true);

                _players.Add (authData, battlePlayer);
            }

            _networkService.InitializeBattlePlayer (battlePlayer);

            _logger.Information ("OnClientLevelLoaded _battleStarted {BattleStarted}", _battleStarted);
            if (_battleStarted)
            {
                JoinBattle (battlePlayer);
                return;
            }

            _logger.Information ("OnClientLevelLoaded _players.Count {PlayersCount}", _players.Count);
            if (_players.Count == 2)
                StartBattle ();
        }

        private void StopBattle ()
        {
            _logger.Information ("StopBattle _battleStarted {BattleStarted}", _battleStarted);

            if (!_battleStarted)
                return;

            _battleStarted = false;

            foreach (var unit in _units.SelectMany (x => x.Value))
            {
                unit
                    .Self
                    .StopBattle ();
            }

            _battleDisposables.Clear ();
        }

        private void StartBattle ()
        {
            _battleStarted = true;

            var gameData = _gameService.GetCachedGameData ();
            var firstPlayer = _players.Values.First ();
            var secondPlayer = _players.Values.Last ();

            void StartBattleForPlayer (Player player, Player opponent)
            {
                _networkService.StartBattle (
                    player,
                    new BattleStartedData
                    {
                        CountdownDelay = gameData.MatchCountdownDelay,
                        Countdown = gameData.MatchCountdown,
                        MatchDuration = gameData.MatchDuration,
                        InitialEnergy = gameData.InitialEnergy,
                        EnergyGrowRate = gameData.EnergyGrowRates[0].Rate,
                        ForfeitPossible = false,
                        OpponentId = opponent.Id,
                        OpponentName = opponent.DisplayName,
                        OpponentLevel = opponent.Level,
                        OpponentRating = opponent.Rating
                    });
            }

            StartBattleForPlayer (firstPlayer, secondPlayer);
            StartBattleForPlayer (secondPlayer, firstPlayer);

            void OnBattleSyncTick (long _)
            {
                var now = DateTimeOffset.Now.ToUnixTimeSeconds ();

                foreach (var player in _players.Values)
                {
                    if (now - player.EmojiSetTime >= gameData.EmojiDuration)
                        player.Emoji = null;
                }

                _networkService.SyncBattle (
                    _players.Values,
                    GetDuration (),
                    _overtime ? gameData.OvertimeDuration : null);
            }

            void AdjustGrowRate (EnergyGrowRate energyGrowRate)
            {
                foreach (var player in _players.Values)
                {
                    player.Energy.SetGrowRate (energyGrowRate.Rate);
                }

                _networkService.AdjustGrowRate (energyGrowRate);
            }

            void OnCountdownComplete (long _)
            {
                _battleStartTime = _timeProvider.Time;
                foreach (var player in _players.Values)
                {
                    player
                        .Energy
                        .SetGrowRate (gameData.EnergyGrowRates[0].Rate);

                    SpawnBaseUnits (player);
                }

                Observable
                    .Interval (TimeSpan.FromSeconds (_args.BattleSyncInterval))
                    .Subscribe (OnBattleSyncTick)
                    .AddTo (_battleDisposables);

                Observable
                    .Timer (TimeSpan.FromSeconds (gameData.ForfeitPossibleDelay))
                    .Subscribe (_ => _networkService.ForfeitPossible (true))
                    .AddTo (_battleDisposables);

                Observable
                    .Timer (TimeSpan.FromSeconds (gameData.MatchDuration))
                    .Subscribe (_ => ReportBattleResult (true))
                    .AddTo (_battleDisposables);

                foreach (var energyGrowRate in gameData.EnergyGrowRates.Skip (1))
                {
                    Observable
                        .Timer (TimeSpan.FromSeconds (energyGrowRate.From))
                        .Subscribe (_ => AdjustGrowRate (energyGrowRate))
                        .AddTo (_battleDisposables);
                }
            }

            foreach (var @base in _levelData.Bases)
            {
                @base.ShipFlyController.LandShip ();
            }

            Observable
                .Timer (TimeSpan.FromSeconds (gameData.MatchCountdown + gameData.MatchCountdownDelay))
                .Subscribe (OnCountdownComplete)
                .AddTo (_battleDisposables);

            _networkService.UnitSpawning.Subscribe (OnUnitSpawning).AddTo (_battleDisposables);
            _networkService.PlayerSetEmoji.Subscribe (OnPlayerSetEmoji).AddTo (_battleDisposables);
            _networkService.PlayerForfeiting.Subscribe (OnPlayerForfeiting).AddTo (_battleDisposables);
        }

        private void JoinBattle (Player player)
        {
            _logger.Information ("JoinBattle id {Id}", player.Id);

            var gameData = _gameService.GetCachedGameData ();
            var opponent = _players.Values.FirstOrDefault (x => x.Id != player.Id);
            if (opponent == null)
                throw new InvalidOperationException ("Opponent was not found while trying to join the battle");

            _reconnectionTimer?.Dispose ();
            _networkService.JoinBattle (
                player,
                new BattleJoinData
                {
                    InitialEnergy = gameData.InitialEnergy,
                    EnergyGrowRate = GetEnergyGrowRate (),
                    Duration = GetDuration (),
                    MatchDuration = gameData.MatchDuration,
                    OvertimeDuration = _overtime ? gameData.OvertimeDuration : null,
                    ForfeitPossible = GetDuration () >= gameData.ForfeitPossibleDelay,
                    OpponentId = opponent.Id,
                    OpponentName = opponent.DisplayName,
                    OpponentRating = opponent.Rating,
                    OpponentLevel = opponent.Level,
                    PlayerEmoji = player.Emoji,
                    OpponentEmoji = opponent.Emoji
                });
        }

        private void StartOvertime ()
        {
            _overtime = true;

            var gameData = _gameService.GetCachedGameData ();
            Observable
                .Timer (TimeSpan.FromSeconds (gameData.OvertimeDuration))
                .Subscribe (_ => ReportBattleResult (false))
                .AddTo (_battleDisposables);

            _networkService.StartOvertime ();
        }

        private bool HandleReportBattleResultEdgeCases (string singlePlayerMessage)
        {
            var onlinePlayers = _players.Where (x => x.Value.Online).ToList ();
            switch (onlinePlayers.Count)
            {
                case 0:
                    return true;
                case 1:
                    var winner = onlinePlayers.First ().Value;
                    var opponent = _players.Values.First (x => x.Id != winner.Id);
                    _gameSessionProvider.GameSession.WinnerId = winner.Id;
                    _networkService.ReportBattleResult ((winner, opponent.Id, BattleResult.Victory, singlePlayerMessage));
                    return true;
                case 2:
                    return false;
                default:
                    throw new InvalidOperationException ($"Invalid players number {onlinePlayers.Count}");
            }
        }

        private void ReportBattleResult (PlayerTeam winnerTeam)
        {
            StopBattle ();

            if (HandleReportBattleResultEdgeCases (OpponentLeftMessage))
                return;

            var players = _players
                .Where (x => x.Value.Online)
                .Select (
                    x =>
                    {
                        var isWinner = x.Value.Team == winnerTeam;
                        var opponent = _players.Values.First (y => y.Id != x.Value.Id);
                        return isWinner
                            ? (x.Value, opponent.Id, BattleResult.Victory, OpponentDefeatedMessage)
                            : (x.Value, opponent.Id, BattleResult.Defeat, YouDefeatedMessage);
                    })
                .ToArray ();

            var gameSession = _gameSessionProvider.GameSession;
            gameSession.WinnerId = players.First (x => x.Item3 == BattleResult.Victory).Value.Id;

            _networkService.ReportBattleResult (players);
        }

        private void ReportBattleResult (bool overtimePossible)
        {
            if (!overtimePossible && HandleReportBattleResultEdgeCases (OpponentDefeatedMessage))
            {
                StopBattle ();
                return;
            }

            (Player winner, Player looser)? GetWeakestBaseUnitHpWinner ()
            {
                (Player player, int score)[] _scores = _units
                    .Select (
                        x => (_players.First (y => y.Value.Team == x.Key).Value,
                            x.Value.Min (
                                y => y.Self is BaseUnitNetworkBehaviour baseUnit
                                    ? baseUnit.Hp.Value > 0 ? baseUnit.Hp.Value : int.MaxValue
                                    : int.MaxValue)))
                    .ToArray ();

                /*Logger.Log (
                    this,
                    $"GetWeakestBaseUnitHpWinner {string.Join (", ", _scores.Select (x => $"{x.player.Team}: {x.score}"))}");*/

                return _scores[0].score.CompareTo (_scores[1].score) switch
                {
                    1 => (_scores[0].player, _scores[1].player),
                    -1 => (_scores[1].player, _scores[0].player),
                    _ => null
                };
            }

            (Player winner, Player looser)? GetBaseUnitsCountWinner ()
            {
                (Player player, int score)[] _scores = _units
                    .Select (
                        x => (_players.First (y => y.Value.Team == x.Key).Value,
                            x.Value.Count (y => y.Self is BaseUnitNetworkBehaviour)))
                    .ToArray ();

                /*Logger.Log (
                    this,
                    $"GetBaseUnitsCountWinner {string.Join (", ", _scores.Select (x => $"{x.player.Team}: {x.score}"))}");*/

                return _scores[0].score.CompareTo (_scores[1].score) switch
                {
                    1 => (_scores[0].player, _scores[1].player),
                    -1 => (_scores[1].player, _scores[0].player),
                    _ => null
                };
            }

            var players = GetBaseUnitsCountWinner ();
            if (overtimePossible && !players.HasValue)
            {
                StartOvertime ();
                return;
            }

            StopBattle ();

            do
            {
                if (players.HasValue)
                    break;

                players = GetWeakestBaseUnitHpWinner ();
                if (players.HasValue)
                    break;

                var first = _players.Values.First ();
                var last = _players.Values.Last ();

                _networkService.ReportBattleResult (
                    (first, last.Id, BattleResult.Drawn, DrawGameMessage),
                    (last, first.Id, BattleResult.Drawn, DrawGameMessage));

                _gameSessionProvider
                    .GameSession
                    .WinnerId = string.Empty;

                return;
            } while (false);

            _gameSessionProvider
                .GameSession
                .WinnerId = players.Value.winner.Id;

            _networkService.ReportBattleResult (
                (players.Value.winner, players.Value.looser.Id, BattleResult.Victory, OpponentDefeatedMessage),
                (players.Value.looser, players.Value.winner.Id, BattleResult.Defeat, YouDefeatedMessage));
        }

        private async UniTask OnSpawnUnitAsync (AuthData authData, UnitSpawningData unitSpawningData)
        {
            var player = GetPlayer (authData);
            var gameData = _gameService.GetCachedGameData ();
            var card = unitSpawningData.Card;
            var cardData = gameData.GetCard (card.Id);
            var requiredEnergy = cardData.Energy;

            player.Deck.UseCard (cardData.Id);
            player.Energy.Spend (requiredEnergy);

            var playerTeam = player.Team;
            var playerBase = _levelData.GetBase (playerTeam);
            var skinId = card.Skin;
            var unitData = gameData.GetUnit (skinId);
            var unitPosition = unitSpawningData.Position;
            var unitNode = AstarPath.active.GetNearest (unitPosition, NNConstraint.Default);
            unitPosition = unitNode.position;

            var batchSize = cardData.BatchSize;
            var batchOffsets = cardData.BatchOffsets;
            if (batchOffsets.Length != batchSize)
                throw new InvalidOperationException ("Batch size and offsets number mismatch");

            var batchOffsetsMatrix = Matrix4x4.Rotate (_levelData.GetBase (playerTeam).SpawnRotation);

            for (var i = 0; i < batchSize; i++)
            {
                var batchOffset = batchOffsetsMatrix.MultiplyPoint3x4 (new Vector3 (batchOffsets[i].x, 0f, batchOffsets[i].y));
                var unit = await _unitFactory.Create (
                    unitData,
                    player.GetPlayerCard (card.Id).Level.Value,
                    player.GetBoost (unitData.BoostId),
                    unitPosition + batchOffset,
                    playerBase.SpawnRotation);
                unit.SetTeam (playerTeam);

                AddUnits (playerTeam, unit);
                _networkService.SpawnUnit (unit);
            }

            _networkService.ReplaceNextCard (
                player,
                new NextCardReplacement
                {
                    UsedCard = unitSpawningData.Card,
                    NextCard = player.Deck.NextCard
                });
        }

        private async void OnUnitSpawning ((AuthData, UnitSpawningData) _)
        {
            var (id, unitSpawningData) = _;

            _logger.Information ("OnUnitSpawning id: {Id}; unitSpawningData: {UnitSpawningData}", id, unitSpawningData.Position);

            await OnSpawnUnitAsync (id, unitSpawningData);
        }

        private void OnPlayerSetEmoji ((AuthData, string) _)
        {
            var (authData, emoji) = _;
            var player = GetPlayer (authData);
            player.Emoji = emoji;
            player.EmojiSetTime = DateTimeOffset.Now.ToUnixTimeSeconds ();
        }

        private void OnPlayerForfeiting (AuthData authData)
        {
            _logger.Information ("OnPlayerForfeiting id: {Id}", authData.PlayerId);

            var gameSession = _gameSessionProvider.GameSession;
            gameSession.WinnerId = gameSession.Players.First (x => x.Id != authData.PlayerId).Id;

            var opponent = _players
                .FirstOrDefault (x => !x.Key.Equals (authData) && x.Value.Online)
                .Value;

            _logger.Information (
                "OnPlayerForfeiting _players: {Players}, opponent is not null: {Opponent}",
                string.Join (", ", _players.Values.Select (x => $"Id: {x.Id}; Online: {x.Online}")),
                opponent != null);

            if (opponent != null)
                _networkService.ReportBattleResult (
                    (opponent, _players[authData].Id, BattleResult.Victory, OpponentForfeitedMessage));

            StopBattle ();
        }

        private void OnClientDisconnected (AuthData authData)
        {
            _logger.Information (
                "OnClientDisconnected _battleStarted {BattleStarted} authData {AuthData}",
                _battleStarted,
                authData);
            if (!_battleStarted)
                return;

            var player = GetPlayer (authData, true);
            if (player != null)
                player.Online = false;

            if (_players.Values.Count (x => x.Online) == 1)
            {
                _logger.Information ("Starting the reconnection timer...");
                _reconnectionTimer = Observable
                    .Timer (TimeSpan.FromSeconds (_gameService.GetCachedGameData ().ReconnectionTimeout))
                    .Subscribe (
                        _ =>
                        {
                            _logger.Information ("Reconnection timer fired _players.Count {PlayersCount}", _players.Count);
                            StopBattle ();
                            if (_players.Count (x => x.Value.Online) == 1)
                                ReportBattleResult (_players.First (x => x.Value.Online).Value.Team);
                        })
                    .AddTo (_battleDisposables);
            }
        }

        public void Initialize ()
        {
            _networkService.ClientLevelLoaded.Subscribe (OnClientLevelLoaded).AddTo (_disposables);
            _networkService.ClientDisconnected.Subscribe (OnClientDisconnected).AddTo (_disposables);
        }

        public void Dispose ()
        {
            StopBattle ();

            _players.Clear ();
            _units.Clear ();

            _disposables.Dispose ();
            _battleDisposables.Dispose ();
        }

        public void GetTargetsFor (IUnit unit, List<ITarget> targets)
        {
            GetTargetsFor (unit.Team, targets);
        }

        public void GetTargetsFor (PlayerTeam team, List<ITarget> targets)
        {
            targets.Clear ();

            var oppositePosition = team.GetOpposite ();
            if (_units.TryGetValue (oppositePosition, out var oppositeUnits))
                targets.AddRange (
                    oppositeUnits
                        .Where (x => x.Self is { IsDying: false })
                        .Select (x => x.Target));
        }
        #else
        public void Initialize ()
        {
        }

        public void Dispose ()
        {
        }
        
        public void GetTargetsFor (IUnit unit, List<ITarget> targets)
        {
        }
        
        public void GetTargetsFor (PlayerTeam team, List<ITarget> targets)
        {
        }
        #endif
    }
}