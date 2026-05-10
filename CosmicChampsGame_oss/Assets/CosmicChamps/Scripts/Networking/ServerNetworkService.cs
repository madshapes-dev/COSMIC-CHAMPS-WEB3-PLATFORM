using System;
using System.Collections.Generic;
using System.Linq;
using CosmicChamps.Battle;
using CosmicChamps.Battle.Units;
using CosmicChamps.Data;
using CosmicChamps.Networking.Messages;
using Cysharp.Threading.Tasks;
using kcp2k;
using Mirror;
using Mirror.SimpleWeb;
using Serilog;
using ThirdParty.Extensions.Components;
using BattleData = CosmicChamps.Battle.Data;
using Object = UnityEngine.Object;
using Player = CosmicChamps.Battle.Data.Server.Player;

namespace CosmicChamps.Networking
{
    #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
    public class ServerNetworkService
    {
        public readonly struct Args
        {
            public readonly NetworkIdentity PlayerPrefab;
            public readonly int MaxConnections;

            public Args (NetworkIdentity playerPrefab, int maxConnections)
            {
                PlayerPrefab = playerPrefab;
                MaxConnections = maxConnections;
            }
        }

        private readonly ObservableEvent<AuthData> _clientAuthenticated = new();
        private readonly ObservableEvent<AuthData> _clientLevelLoaded = new();
        private readonly ObservableEvent<AuthData> _clientDisconnected = new();
        private readonly ObservableEvent<(AuthData, BattleData.UnitSpawningData)> _unitSpawning = new();
        private readonly ObservableEvent<AuthData> _playerForfeiting = new();
        private readonly ObservableEvent<(AuthData, string)> _playerSetEmoji = new();

        private readonly Args _args;
        private readonly KcpTransport _transport;
        private readonly SimpleWebTransport _webGLTransport;
        private readonly GameLiftServer _gameLiftServer;
        private readonly Authenticator _authenticator;
        private readonly ILogger _logger;

        public IObservable<AuthData> ClientAuthenticated => _clientAuthenticated.AsObservable ();
        public IObservable<AuthData> ClientLevelLoaded => _clientLevelLoaded.AsObservable ();
        public IObservable<AuthData> ClientDisconnected => _clientDisconnected.AsObservable ();
        public IObservable<(AuthData, BattleData.UnitSpawningData)> UnitSpawning => _unitSpawning.AsObservable ();
        public IObservable<AuthData> PlayerForfeiting => _playerForfeiting.AsObservable ();
        public IObservable<(AuthData, string)> PlayerSetEmoji => _playerSetEmoji.AsObservable ();

        public ServerNetworkService (
            Args args,
            KcpTransport transport,
            SimpleWebTransport webGLTransport,
            Authenticator authenticator,
            GameLiftServer gameLiftServer,
            ILogger logger)
        {
            _args = args;
            _transport = transport;
            _webGLTransport = webGLTransport;
            _authenticator = authenticator;
            _gameLiftServer = gameLiftServer;
            _logger = logger;
        }

        private IEnumerable<NetworkConnectionToClient> GetConnections () => NetworkServer.connections.Values;

        private void OnClientLevelLoaded (NetworkConnectionToClient conn, ClientLevelLoadedMessage message)
        {
            var authData = conn.GetAuthData ();
            _logger.Information ("OnClientLevelLoaded {AuthData}", authData);
            _clientLevelLoaded.Fire (authData);
        }

        private void OnServerConnect (NetworkConnectionToClient conn)
        {
            _logger.Information ("OnServerConnect {ConnAddress}", conn.address);
            _authenticator.OnServerAuthenticate (conn);
        }

        private void OnServerAuthenticated (NetworkConnectionToClient conn)
        {
            var authData = conn.GetAuthData ();
            _logger.Information ("OnServerAuthenticated {AuthData}", authData);
            conn.isAuthenticated = true;
            _gameLiftServer.AcceptPlayerSession (authData.PlayerSessionId);
            _clientAuthenticated.Fire (authData);
        }

        private void OnServerDisconnect (NetworkConnectionToClient conn)
        {
            if (!conn.isAuthenticated)
                return;

            var authData = conn.GetAuthData ();
            _logger.Information ("OnServerDisconnect {AuthData}", authData);
            _gameLiftServer.RemovePlayerSession (authData.PlayerSessionId);
            NetworkServer.DestroyPlayerForConnection (conn);
            _clientDisconnected.Fire (authData);

            if (!GetConnections ().Any ())
                Shutdown ();
        }

        private void OnServerError (NetworkConnectionToClient conn, TransportError error, string reason)
        {
            _logger.Error (
                "OnServerError connectionId: {ConnConnectionId}; error: {Error}; reason {Reason}",
                conn?.connectionId,
                error,
                reason);

            switch (error)
            {
                case TransportError.DnsResolve:
                case TransportError.Refused:
                case TransportError.Timeout:
                case TransportError.Congestion:
                case TransportError.ConnectionClosed:
                    break;
                case TransportError.InvalidReceive:
                case TransportError.InvalidSend:
                case TransportError.Unexpected:
                    Shutdown ();
                    break;
                default:
                    throw new ArgumentOutOfRangeException (nameof (error), error, null);
            }
        }

        private void OnUnitSpawning (NetworkConnectionToClient conn, UnitSpawningMessage message)
        {
            _logger.Information (
                "OnUnitSpawning {ConnConnectionId}; card: {Card}; position {Position}",
                conn.connectionId,
                message.UnitSpawningData.Card,
                message.UnitSpawningData.Position);
            _unitSpawning.Fire ((conn.GetAuthData (), message.UnitSpawningData));
        }

        private void OnForfeit (NetworkConnectionToClient conn, ForfeitMessage _)
        {
            _playerForfeiting.Fire (conn.GetAuthData ());
        }

        private void OnSetEmoji (NetworkConnectionToClient conn, SetEmojiMessage message)
        {
            var authData = conn.GetAuthData ();
            _playerSetEmoji.Fire ((authData, message.Emoji));
        }

        private void CreatePlayerForConnection (NetworkConnectionToClient conn)
        {
            var authData = conn.GetAuthData ();
            _logger.Information ("CreatePlayerForConnection {AuthData}", authData);
            var player = Object.Instantiate (_args.PlayerPrefab);
            player.name = $"Player [Id={authData.PlayerId}]";
            NetworkServer.AddPlayerForConnection (conn, player.gameObject);
        }

        private NetworkConnectionToClient GetPlayerConnection (Player player)
        {
            var conn = GetConnections ().FirstOrDefault (x => x.GetAuthData ()?.PlayerId == player.Id);
            if (conn == null)
                throw new ArgumentException ($"Unable to find connection for player {player.Id}");

            return conn;
        }

        private NetworkConnectionToClient GetPlayerConnection (AuthData authData)
        {
            var conn = GetConnections ().FirstOrDefault (x => x.GetAuthData ()?.PlayerId == authData.PlayerId);
            if (conn == null)
                throw new ArgumentException ($"Unable to find connection for player {authData.PlayerId}");

            return conn;
        }

        public void BattleStarting (string level, PlayerDeckCard[] cards)
        {
            _logger.Information ("BattleStarting");
            foreach (var connection in GetConnections ())
            {
                connection.Send (
                    new BattleStartingMessage
                    {
                        BattleStartingData = new BattleData.BattleStartingData
                        {
                            Level = level,
                            Cards = cards
                                .Select (
                                    x => new BattleData.Card
                                    {
                                        Id = x.Id,
                                        Skin = x.Skin
                                    })
                                .ToArray ()
                        }
                    });
            }
        }

        public void BattleStarting (AuthData authData, string level, PlayerDeckCard[] cards)
        {
            _logger.Information ("BattleStarting {AuthData}", authData);

            var conn = GetPlayerConnection (authData);
            conn.Send (
                new BattleStartingMessage
                {
                    BattleStartingData = new BattleData.BattleStartingData
                    {
                        Level = level,
                        Cards = cards
                            .Select (
                                x => new BattleData.Card
                                {
                                    Id = x.Id,
                                    Skin = x.Skin
                                })
                            .ToArray ()
                    }
                });
        }

        public void Start (ushort port)
        {
            if (NetworkServer.active)
                return;

            _transport.Port = port;
            _webGLTransport.port = (ushort)(port + 2);

            NetworkServer.Listen (_args.MaxConnections);

            _authenticator.OnStartServer ();
            _authenticator.OnServerAuthenticated.AddListener (OnServerAuthenticated);

            NetworkServer.OnConnectedEvent = OnServerConnect;
            NetworkServer.OnDisconnectedEvent = OnServerDisconnect;
            NetworkServer.OnErrorEvent = OnServerError;

            NetworkServer.RegisterHandler<ClientLevelLoadedMessage> (OnClientLevelLoaded);
            NetworkServer.RegisterHandler<UnitSpawningMessage> (OnUnitSpawning);
            NetworkServer.RegisterHandler<ForfeitMessage> (OnForfeit);
            NetworkServer.RegisterHandler<SetEmojiMessage> (OnSetEmoji);
        }

        public void InitializeBattlePlayer (Player player)
        {
            _logger.Information ("InitializeBattlePlayer {PlayerId}", player.Id);
            var conn = GetPlayerConnection (player);

            conn.Send (
                new BattleInitializedMessage
                {
                    BattleInitData = new BattleData.BattleInitData
                    {
                        PlayerId = conn.GetAuthData ().PlayerId,
                        PlayerTeam = player.Team,
                        PlayerDeck = new BattleData.Deck
                        {
                            Cards = player.Deck.Cards.ToArray (),
                            NextCard = player.Deck.NextCard
                        }
                    }
                });
        }

        public void StartBattle (Player player, BattleData.BattleStartedData battleStartedData)
        {
            _logger.Information ("StartBattle");

            var conn = GetPlayerConnection (player);
            CreatePlayerForConnection (conn);

            conn.Send (
                new BattleStartedMessage
                {
                    BattleStartedData = battleStartedData
                });
        }

        public void StartOvertime ()
        {
            _logger.Information ("StartOvertime");
            NetworkServer.SendToAll (new BattleOvertimeMessage ());
        }

        public void JoinBattle (Player player, BattleData.BattleJoinData battleJoinData)
        {
            _logger.Information ("JoinBattle {PlayerId}", player.Id);
            var conn = GetPlayerConnection (player);
            CreatePlayerForConnection (conn);
            conn.Send (
                new BattleJoinedMessage
                {
                    BattleJoinData = battleJoinData
                });
        }

        public void ForfeitPossible (bool possible)
        {
            NetworkServer.SendToAll (new ForfeitPossibleMessage { Possible = possible });
        }

        public void SyncBattle (ICollection<Player> players, float time, float? overtimeDuration)
        {
            foreach (var player in players.Where (x => x.Online))
            {
                var conn = GetPlayerConnection (player);
                conn.Send (
                    new BattleSyncMessage
                    {
                        BattleSyncData = new BattleData.BattleSyncData
                        {
                            EnergyValue = player.Energy.Value,
                            EnergyGrowRate = player.Energy.GrowRate,
                            MatchDuration = time,
                            OvertimeDuration = overtimeDuration,
                            PlayerEmoji = player.Emoji,
                            OpponentEmoji = players.FirstOrDefault (x => x != player)?.Emoji
                        }
                    });
            }
        }

        public void AdjustGrowRate (EnergyGrowRate energyGrowRate)
        {
            _logger.Information ("AdjustGrowRate");
            NetworkServer.SendToAll (
                new AdjustEnergyGrowRateMessage
                {
                    EnergyGrowRate = energyGrowRate
                });
        }

        public void SpawnUnit (RegularUnitNetworkBehaviour unit)
        {
            _logger.Information ("SpawnUnit");
            NetworkServer.Spawn (unit.gameObject);
        }

        public void SpawnBaseUnit (BaseUnitNetworkBehaviour baseUnit)
        {
            _logger.Information ("SpawnBaseUnit");
            NetworkServer.Spawn (baseUnit.gameObject);
        }

        public void ReplaceNextCard (Player player, BattleData.NextCardReplacement replacement)
        {
            _logger.Information (
                "ReplaceNextCard {PlayerId} {ReplacementUsedCard} {ReplacementNextCard}",
                player.Id,
                replacement.UsedCard,
                replacement.NextCard);

            var conn = GetPlayerConnection (player);
            conn.Send (
                new NextCardReplacedMessage
                {
                    NextCardReplacement = replacement
                });
        }

        public void ReportBattleResult (
            params (Player player, string opponentId, BattleResult result, string message)[] playerResults)
        {
            _logger.Information (
                "ReportBattleResult {Join}",
                string.Join (", ", playerResults.Select (x => $"{x.player.Id}: {x.result}")));

            foreach (var playerResult in playerResults)
            {
                var conn = GetPlayerConnection (playerResult.player);
                conn.Send (
                    new BattleFinishedMessage
                    {
                        BattleFinishData = new BattleData.BattleFinishData
                        {
                            Result = playerResult.result,
                            Message = playerResult.message,
                            OpponentId = playerResult.opponentId
                        }
                    });
            }

            /*async UniTaskVoid DelayedShutdown ()
            {
                _logger.Information ("ReportBattleResult DelayedShutdown delay...");
                await UniTask.Delay (TimeSpan.FromSeconds (5f));
                _logger.Information ("ReportBattleResult DelayedShutdown");
                Shutdown ();
            }

            DelayedShutdown ().Forget ();*/
        }

        public void Shutdown ()
        {
            async UniTaskVoid ShutdownAsync ()
            {
                _logger.Information ("Shutdown");

                _logger.Information ("TerminateGameSession...");
                await _gameLiftServer.TerminateGameSessionAsync (false);

                _logger.Information ("_authenticator.OnStopServer...");
                _authenticator.OnStopServer ();
                _authenticator.OnServerAuthenticated.RemoveListener (OnServerAuthenticated);

                _logger.Information ("NetworkServer.Shutdown...");
                NetworkServer.Shutdown ();

                _logger.Information ("Shutdown Done");
                
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                _logger.Information ("Quit delay...");
                await UniTask.Delay (TimeSpan.FromSeconds (5f));
                _logger.Information ("Quit");
                UnityEngine.Application.Quit(0);
                #endif
            }

            ShutdownAsync ().Forget ();
        }
    }
    #endif
}