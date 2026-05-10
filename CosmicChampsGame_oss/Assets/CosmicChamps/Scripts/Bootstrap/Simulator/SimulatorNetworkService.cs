using System;
using CosmicChamps.Battle;
using CosmicChamps.Battle.PVE;
using CosmicChamps.Data;
using CosmicChamps.Networking;
using CosmicChamps.Networking.Messages;
using CosmicChamps.Services;
using Mirror;
using ThirdParty.Extensions.Components;
using UniRx;
using UnityEngine;
using BattleData = CosmicChamps.Battle.Data;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Bootstrap.Simulator
{
    public class SimulatorNetworkService : IBotNetworkService, IDisposable
    {
        private readonly ObservableEvent<Unit> _battleStarting = new();
        private readonly ObservableEvent<BattleData.BattleInitData> _battleInitialized = new();
        private readonly ObservableEvent<BattleData.NextCardReplacement> _nextCardReplaced = new();
        private readonly ObservableEvent<BattleData.BattleStartedData> _battleStarted = new();
        private readonly ObservableEvent<Unit> _battleOvertime = new();
        private readonly ObservableEvent<BattleData.BattleJoinData> _battleJoined = new();
        private readonly ObservableEvent<BattleData.BattleSyncData> _battleSynced = new();
        private readonly ObservableEvent<bool> _forfeitPossible = new();
        private readonly ObservableEvent<EnergyGrowRate> _energyGrowRateAdjusted = new();
        private readonly ObservableEvent<BattleData.BattleFinishData> _battleFinished = new();

        private readonly ILogger _logger;
        private readonly IGameService _gameService;
        private readonly Authenticator _authenticator;

        public IObservable<Unit> BattleStarting => _battleStarting.AsObservable ();
        public IObservable<BattleData.BattleInitData> BattlePlayerInitialized => _battleInitialized.AsObservable ();
        public IObservable<BattleData.NextCardReplacement> NextCardReplaced => _nextCardReplaced.AsObservable ();
        public IObservable<BattleData.BattleStartedData> BattleStarted => _battleStarted.AsObservable ();
        public IObservable<Unit> BattleOvertime => _battleOvertime.AsObservable ();
        public IObservable<BattleData.BattleJoinData> BattleJoined => _battleJoined.AsObservable ();
        public IObservable<BattleData.BattleSyncData> BattleSynced => _battleSynced.AsObservable ();
        public IObservable<bool> ForfeitPossible => _forfeitPossible.AsObservable ();
        public IObservable<EnergyGrowRate> EnergyGrowRateAdjusted => _energyGrowRateAdjusted.AsObservable ();
        public IObservable<BattleData.BattleFinishData> BattleFinished => _battleFinished.AsObservable ();

        public SimulatorNetworkService (ILogger logger, IGameService gameService, Authenticator authenticator)
        {
            _logger = logger;
            _gameService = gameService;
            _authenticator = authenticator;
        }

        private void OnBattleStarting (BattleStartingMessage _)
        {
            _logger.Information ("OnBattleStarting");
            _battleStarting.Fire (Unit.Default);
        }

        private void OnBattleInitialized (BattleInitializedMessage message)
        {
            _logger.Information ("OnBattleInitialized");
            _battleInitialized.Fire (message.BattleInitData);
        }

        private void OnBattleStarted (BattleStartedMessage message)
        {
            _logger.Information ("OnBattleStarted");
            _battleStarted.Fire (message.BattleStartedData);
        }

        private void OnBattleOvertime (BattleOvertimeMessage message)
        {
            _logger.Information ("OnBattleOvertime");
            _battleOvertime.Fire (Unit.Default);
        }

        private void OnBattleJoined (BattleJoinedMessage message)
        {
            _logger.Information ("OnBattleJoined");
            _battleJoined.Fire (message.BattleJoinData);
        }

        private void OnNextCardReplaced (NextCardReplacedMessage message)
        {
            _logger.Information ("OnNextCardReplaced");
            _nextCardReplaced.Fire (message.NextCardReplacement);
        }

        private void OnBattleSynced (BattleSyncMessage message)
        {
            // _logger.Information ("OnBattleSynced");
            _battleSynced.Fire (message.BattleSyncData);
        }

        private void OnForfeitPossible (ForfeitPossibleMessage message)
        {
            _logger.Information ("OnForfeitPossible");
            _forfeitPossible.Fire (message.Possible);
        }

        private void OnEnergyGrowRateAdjusted (AdjustEnergyGrowRateMessage message)
        {
            _logger.Information ("OnEnergyGrowRateAdjusted");
            _energyGrowRateAdjusted.Fire (message.EnergyGrowRate);
        }

        private void OnBattleFinished (BattleFinishedMessage message)
        {
            _logger.Information ("OnBattleFinished");
            _battleFinished.Fire (message.BattleFinishData);
        }

        private void OnClientConnect ()
        {
            _logger.Information ("OnClientConnect");
            _authenticator.OnClientAuthenticate ();
        }

        private void OnClientAuthenticated ()
        {
            _logger.Information (
                "OnClientAuthenticated {ConnectionAuthenticationData}",
                NetworkClient.connection.authenticationData);

            NetworkClient.connection.isAuthenticated = true;
        }

        private void OnClientDisconnect ()
        {
            _logger.Information ("OnClientDisconnect");
            Application.Quit ();
        }

        private void OnClientError (TransportError error, string reason)
        {
            _logger.Information ("OnClientError error: {Error}; reason: {Reason}", error, reason);
            Application.Quit ();
        }

        private void RegisterMessageHandlers ()
        {
            NetworkClient.OnConnectedEvent = OnClientConnect;
            NetworkClient.OnDisconnectedEvent = OnClientDisconnect;
            NetworkClient.OnErrorEvent = OnClientError;

            NetworkClient.RegisterHandler<BattleStartingMessage> (OnBattleStarting);
            NetworkClient.RegisterHandler<BattleInitializedMessage> (OnBattleInitialized);
            NetworkClient.RegisterHandler<NextCardReplacedMessage> (OnNextCardReplaced);
            NetworkClient.RegisterHandler<BattleStartedMessage> (OnBattleStarted);
            NetworkClient.RegisterHandler<BattleOvertimeMessage> (OnBattleOvertime);
            NetworkClient.RegisterHandler<BattleJoinedMessage> (OnBattleJoined);
            NetworkClient.RegisterHandler<BattleSyncMessage> (OnBattleSynced);
            NetworkClient.RegisterHandler<ForfeitPossibleMessage> (OnForfeitPossible);
            NetworkClient.RegisterHandler<AdjustEnergyGrowRateMessage> (OnEnergyGrowRateAdjusted);
            NetworkClient.RegisterHandler<BattleFinishedMessage> (OnBattleFinished);
        }

        private void OverrideDefaultNetworkClientHandlers ()
        {
            NetworkClient.ReplaceHandler<ObjectDestroyMessage> (OnObjectDestroy);
            NetworkClient.ReplaceHandler<ObjectHideMessage> (OnObjectHide);
            NetworkClient.ReplaceHandler<SpawnMessage> (OnSpawn);
            NetworkClient.ReplaceHandler<ObjectSpawnStartedMessage> (OnObjectSpawnStarted);
            NetworkClient.ReplaceHandler<ObjectSpawnFinishedMessage> (OnObjectSpawnFinished);
            NetworkClient.ReplaceHandler<EntityStateMessage> (OnEntityStateMessage);
            NetworkClient.ReplaceHandler<TimeSnapshotMessage> (OnTimeSnapshotMessage);
            NetworkClient.ReplaceHandler<ChangeOwnerMessage> (OnChangeOwner);
            NetworkClient.ReplaceHandler<RpcBufferMessage> (OnRPCBufferMessage);
        }

        private void OnRPCBufferMessage (RpcBufferMessage obj)
        {
        }

        private void OnChangeOwner (ChangeOwnerMessage obj)
        {
        }

        private void OnTimeSnapshotMessage (TimeSnapshotMessage obj)
        {
        }

        private void OnEntityStateMessage (EntityStateMessage obj)
        {
        }

        private void OnObjectSpawnFinished (ObjectSpawnFinishedMessage obj)
        {
        }

        private void OnObjectSpawnStarted (ObjectSpawnStartedMessage obj)
        {
        }

        private void OnSpawn (SpawnMessage obj)
        {
        }

        private void OnObjectHide (ObjectHideMessage obj)
        {
        }

        private void OnObjectDestroy (ObjectDestroyMessage obj)
        {
        }

        public void Dispose ()
        {
            Stop ();
        }

        public void Start (PlayerGameSession playerGameSession)
        {
            if (NetworkClient.active)
                return;

            _logger.Information ("Start");
            var player = _gameService.GetCachedPlayer ();

            _authenticator.OnStartClient ();
            _authenticator.SetAuthData (
                new AuthData
                {
                    PlayerId = player.Id,
                    PlayerSessionId = playerGameSession.PlayerSessionId
                });
            _authenticator.OnClientAuthenticated.AddListener (OnClientAuthenticated);

            RegisterMessageHandlers ();

            _logger.Information ("playerGameSession {IpAddress}:{Port}", playerGameSession.IpAddress, playerGameSession.Port);
            var uriBuilder = new UriBuilder (kcp2k.KcpTransport.Scheme, playerGameSession.IpAddress, playerGameSession.Port);

            _logger.Information ("NetworkClient.Connect {UriBuilderUri}", uriBuilder.Uri);
            NetworkClient.Connect (uriBuilder.Uri);
            OverrideDefaultNetworkClientHandlers ();
        }

        public void Stop ()
        {
            _logger.Information ("Stop");

            NetworkClient.OnConnectedEvent = null;
            NetworkClient.OnDisconnectedEvent = null;
            NetworkClient.OnErrorEvent = null;

            _authenticator.OnStopClient ();
            _authenticator.OnClientAuthenticated.RemoveListener (OnClientAuthenticated);

            NetworkClient.Disconnect ();
            NetworkClient.Shutdown ();
            
            Application.Quit ();
        }

        public void LevelLoaded () => NetworkClient.Send (new ClientLevelLoadedMessage ());

        public void SpawnUnit (BattleData.Card card, Vector3 position) =>
            NetworkClient.Send (
                new UnitSpawningMessage
                {
                    UnitSpawningData = new BattleData.UnitSpawningData
                    {
                        Card = card,
                        Position = position
                    }
                });
    }
}