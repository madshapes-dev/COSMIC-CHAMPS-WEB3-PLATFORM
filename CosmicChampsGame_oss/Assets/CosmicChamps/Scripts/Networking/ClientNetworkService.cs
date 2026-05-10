using System;
using CosmicChamps.Battle;
using CosmicChamps.Battle.Units;
using CosmicChamps.Data;
using CosmicChamps.Networking.Messages;
using CosmicChamps.Signals;
using Mirror;
using Mirror.SimpleWeb;
using ThirdParty.Extensions.Components;
using UniRx;
using UnityEngine;
using BattleData = CosmicChamps.Battle.Data;
using ILogger = Serilog.ILogger;
using Object = UnityEngine.Object;

namespace CosmicChamps.Networking
{
    public class ClientNetworkService : IDisposable
    {
        public readonly struct Args
        {
            public readonly NetworkIdentity PlayerPrefab;

            public Args (NetworkIdentity playerPrefab)
            {
                PlayerPrefab = playerPrefab;
            }
        }

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        private readonly ObservableEvent<BattleData.BattleInitData> _battleInitialized = new();
        private readonly ObservableEvent<BattleData.NextCardReplacement> _nextCardReplaced = new();
        private readonly ObservableEvent<BattleData.BattleStartingData> _battleStarting = new();
        private readonly ObservableEvent<BattleData.BattleStartedData> _battleStarted = new();
        private readonly ObservableEvent<Unit> _battleOvertime = new();
        private readonly ObservableEvent<BattleData.BattleJoinData> _battleJoined = new();
        private readonly ObservableEvent<BattleData.BattleSyncData> _battleSynced = new();
        private readonly ObservableEvent<bool> _forfeitPossible = new();
        private readonly ObservableEvent<EnergyGrowRate> _energyGrowRateAdjusted = new();
        private readonly ObservableEvent<BattleData.BattleFinishData> _battleFinished = new();

        private readonly IMessageBroker _messageBroker;
        private readonly IScenesLoadingService _scenesLoadingService;
        private readonly GameDataRepository _gameDataRepository;
        private readonly Authenticator _authenticator;
        private readonly Args _args;
        private readonly SimpleWebTransport _webGLTransport;
        private readonly ILogger _logger;

        public PlayerGameSession PlayerGameSession { private set; get; }
        public double Rtt => NetworkTime.rtt / 2;
        public bool IsConnected => NetworkClient.isConnected;

        public IObservable<BattleData.BattleInitData> BattlePlayerInitialized => _battleInitialized.AsObservable ();
        public IObservable<BattleData.NextCardReplacement> NextCardReplaced => _nextCardReplaced.AsObservable ();
        public IObservable<BattleData.BattleStartingData> BattleStarting => _battleStarting.AsObservable ();
        public IObservable<BattleData.BattleStartedData> BattleStarted => _battleStarted.AsObservable ();
        public IObservable<Unit> BattleOvertime => _battleOvertime.AsObservable ();
        public IObservable<BattleData.BattleJoinData> BattleJoined => _battleJoined.AsObservable ();
        public IObservable<BattleData.BattleSyncData> BattleSynced => _battleSynced.AsObservable ();
        public IObservable<bool> ForfeitPossible => _forfeitPossible.AsObservable ();
        public IObservable<EnergyGrowRate> EnergyGrowRateAdjusted => _energyGrowRateAdjusted.AsObservable ();
        public IObservable<BattleData.BattleFinishData> BattleFinished => _battleFinished.AsObservable ();

        public ClientNetworkService (
            IMessageBroker messageBroker,
            IScenesLoadingService scenesLoadingService,
            GameDataRepository gameDataRepository,
            Authenticator authenticator,
            Args args,
            SimpleWebTransport webGLTransport,
            ILogger logger)
        {
            _messageBroker = messageBroker;
            _scenesLoadingService = scenesLoadingService;
            _gameDataRepository = gameDataRepository;
            _authenticator = authenticator;
            _args = args;
            _webGLTransport = webGLTransport;
            _logger = logger;
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
            _messageBroker.Publish (new ErrorSignal ("Connection lost", string.Empty, true, false, false));
        }

        private void OnClientError (TransportError error, string reason)
        {
            _messageBroker.Publish (new ErrorSignal (reason, string.Empty, true, false));
        }

        private void OnBattleStarting (BattleStartingMessage message)
        {
            _logger.Information ("OnBattleStarting");
            _battleStarting.Fire (message.BattleStartingData);
        }

        private void OnBattleInitialized (BattleInitializedMessage message)
        {
            _logger.Information ("OnBattlePlayerInitialized");
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
            _battleSynced.Fire (message.BattleSyncData);
        }

        private void OnForfeitPossible (ForfeitPossibleMessage message)
        {
            _forfeitPossible.Fire (message.Possible);
        }

        private void OnEnergyGrowRateAdjusted (AdjustEnergyGrowRateMessage message)
        {
            _energyGrowRateAdjusted.Fire (message.EnergyGrowRate);
        }

        private void OnBattleFinished (BattleFinishedMessage message)
        {
            _battleFinished.Fire (message.BattleFinishData);
        }

        public void LevelLoaded ()
        {
            NetworkClient.Send (new ClientLevelLoadedMessage ());
        }

        public void RegisterUnitPrefabPrefab (
            GameObject prefab,
            Func<Object, Vector3, Quaternion, Transform, RegularUnitNetworkBehaviour> factory)
        {
            NetworkClient.RegisterPrefab (
                prefab,
                msg =>
                {
                    var instance = factory (
                        prefab,
                        msg.position,
                        msg.rotation,
                        null);
                    instance.name = $"{prefab.name} [connId={msg.netId}]";
                    return instance.gameObject;
                },
                Object.Destroy);
        }

        public void Start (PlayerGameSession playerGameSession)
        {
            if (NetworkClient.active)
                return;

            PlayerGameSession = playerGameSession;

            var player = _gameDataRepository.GetCachedPlayer ();

            _authenticator.OnStartClient ();
            _authenticator.SetAuthData (
                new AuthData
                {
                    PlayerId = player.Id,
                    PlayerSessionId = playerGameSession.PlayerSessionId
                });
            _authenticator.OnClientAuthenticated.AddListener (OnClientAuthenticated);

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

            NetworkClient.RegisterPrefab (_args.PlayerPrefab.gameObject);

            _logger.Information ("playerGameSession {IpAddress}:{Port}", playerGameSession.IpAddress, playerGameSession.Port);
            #if UNITY_WEBGL && !UNITY_EDITOR
            _webGLTransport.port = (ushort)(playerGameSession.WebGLPort);
            var uriBuilder = new UriBuilder (Mirror.SimpleWeb.SimpleWebTransport.SecureScheme, playerGameSession.DnsName);
            #else
            var uriBuilder = new UriBuilder (kcp2k.KcpTransport.Scheme, playerGameSession.IpAddress, playerGameSession.Port);
            #endif
            _logger.Information ("NetworkClient.Connect {UriBuilderUri}", uriBuilder.Uri);
            NetworkClient.Connect (uriBuilder.Uri);
        }

        public void Stop ()
        {
            NetworkClient.OnConnectedEvent = null;
            NetworkClient.OnDisconnectedEvent = null;
            NetworkClient.OnErrorEvent = null;

            PlayerGameSession = null;

            _authenticator.OnStopClient ();
            _authenticator.OnClientAuthenticated.RemoveListener (OnClientAuthenticated);

            NetworkClient.Disconnect ();
            NetworkClient.Shutdown ();
        }

        public void SpawnUnit (BattleData.Card card, Vector3 position)
        {
            var message = new UnitSpawningMessage
            {
                UnitSpawningData = new BattleData.UnitSpawningData
                {
                    Card = card,
                    Position = position
                }
            };
            NetworkClient.Send (message);
        }

        public void SetEmoji (string emoji)
        {
            var message = new SetEmojiMessage
            {
                Emoji = emoji
            };
            NetworkClient.Send (message);
        }

        public void Forfeit ()
        {
            NetworkClient.Send (new ForfeitMessage ());
        }

        public void Dispose ()
        {
            Stop ();
        }
        #else
        public void Dispose ()
        {
        }
        #endif
    }
}