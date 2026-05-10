using System;
using System.Collections.Generic;
using System.Linq;
using CosmicChamps.Bootstrap.Server;
using CosmicChamps.Data;
using CosmicChamps.ServerMatchmaking;
using CosmicChamps.Services;
using CosmicChamps.Settings;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using UniRx;
#if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
using Aws.GameLift.Server;
#endif

namespace CosmicChamps.Networking
{
    #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
    public class GameLiftServer : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly GameRepository _gameRepository;
        private readonly IGameService _gameService;
        private readonly GameSessionProvider _gameSessionProvider;
        private readonly PreBattleServiceFactory _preBattleServiceFactory;
        private readonly ILogger _logger;
        private readonly ServerProcessOptions _serverProcessOptions;
        private readonly float _circuitBreakerTimeout;

        private GameSession _gameSession;

        public GameLiftServer (
            GameRepository gameRepository,
            GameSessionProvider gameSessionProvider,
            PreBattleServiceFactory preBattleServiceFactory,
            ILogger logger,
            ServerProcessOptions serverProcessOptions,
            float circuitBreakerTimeout,
            IGameService gameService)
        {
            _gameRepository = gameRepository;
            _gameSessionProvider = gameSessionProvider;
            _preBattleServiceFactory = preBattleServiceFactory;
            _logger = logger;
            _serverProcessOptions = serverProcessOptions;
            _circuitBreakerTimeout = circuitBreakerTimeout;
            _gameService = gameService;
        }

        public void Start (ServerProcessOptions serverProcessOptions)
        {
            var sdkVersion = GameLiftServerAPI.GetSdkVersion ().Result;
            _logger.Information ("SDK VERSION: {SdkVersion}", sdkVersion);

            try
            {
                var initOutcome = GameLiftServerAPI.InitSDK(new ServerParameters
                {
                    WebSocketUrl = null,
                    ProcessId = null,
                    HostId = null,
                    FleetId = null,
                    AuthToken = null,
                    AwsRegion = null,
                    AccessKey = null,
                    SecretKey = null,
                    SessionToken = null
                });

                if (initOutcome.Success)
                {
                    _logger.Information ("SERVER IS IN A GAMELIFT FLEET");
                    ProcessReady ();
                } else
                {
                    _logger.Error (
                        "SERVER NOT IN A FLEET. GameLiftServerAPI.InitSDK() returned{NewLine}{ErrorMessage}",
                        Environment.NewLine,
                        initOutcome.Error.ErrorMessage);
                }
            } catch (Exception e)
            {
                _logger.Error (e, "SERVER NOT IN A FLEET. GameLiftServerAPI.InitSDK() exception");
            }
        }

        public async UniTask TerminateGameSessionAsync (bool appQuit = true)
        {
            var errorCode = 0;
            try
            {
                _logger.Information ("GameLiftServerAPI.ProcessEnding...");
                var outcome = GameLiftServerAPI.ProcessEnding ();
                if (outcome.Success)
                {
                    _logger.Information ("StopGameSession...");
                    if (_gameSession != null)
                    {
                        var winnerId = _gameSession.WinnerId;
                        var winnerWalletId = _gameSession
                            .Players
                            ?.FirstOrDefault (x => x.Id == winnerId)
                            ?.WalletId;
                        _logger.Information (
                            "StopGameSession winnerId {WinnerId} winnerWalletId {WinnerWalletId}",
                            winnerId,
                            winnerWalletId);

                        await _gameRepository.StopGameSession (
                            _gameSession.Id,
                            string.IsNullOrEmpty (winnerId),
                            winnerId,
                            winnerWalletId);
                    }

                    _logger.Information ("StopGameSession Completed");
                } else
                {
                    errorCode = 1;
                    _logger.Error ("PROCESSENDING FAILED. ProcessEnding() returned {Error}", outcome.Error);
                }
            } catch (Exception e)
            {
                errorCode = 1;
                _logger.Error (e, "GAME SESSION TERMINATION FAILED. TerminateGameSession() exception");
            } finally
            {
                _logger.Information ("Almost done, error code: {ErrorCode}", errorCode);
                /*_logger.Information ("Almost done, error code: {ErrorCode}", errorCode);
                _logger.Information ("Backing up logs {LogFilePath}...", _serverProcessOptions.LogFile);
                if (!string.IsNullOrEmpty (_serverProcessOptions.LogFile))
                {
                    string gameSessionId;
                    if (_gameSession != null)
                    {
                        var gameSessionChunks = _gameSession.Id.Split ('/');
                        gameSessionId = gameSessionChunks.Length > 0
                            ? gameSessionChunks[^1]
                            : _gameSession.Id;
                    } else
                    {
                        gameSessionId = "none";
                    }

                    var logBackupPath = Path.Combine (
                        Path.GetDirectoryName (_serverProcessOptions.LogFile) ?? ".",
                        "Logs",
                        $"{gameSessionId}-Pid:{Process.GetCurrentProcess ().Id}-Port:{_serverProcessOptions.Port}-{DateTime.Now:s}.log");

                    if (!File.Exists (logBackupPath))
                    {
                        var logsDirectory = Path.GetDirectoryName (logBackupPath);
                        if (!Directory.Exists (logsDirectory))
                            Directory.CreateDirectory (logsDirectory);

                        _logger.Information ("Backing up logs to {LogBackupPath}", logBackupPath);

                        File.Copy (_serverProcessOptions.LogFile, logBackupPath);
                    }
                }*/

                if (appQuit)
                {
                    await UniTask.Delay (TimeSpan.FromSeconds (0.5));
                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #else
                    await UniTask.SwitchToMainThread ();
                    UnityEngine.Application.Quit (errorCode);
                    #endif
                }
            }
        }

        public void TerminateGameSession (bool appQuit = true)
        {
            TerminateGameSessionAsync (appQuit).Forget ();
        }

        public bool AcceptPlayerSession (string playerSessionId)
        {
            try
            {
                var outcome = GameLiftServerAPI.AcceptPlayerSession (playerSessionId);
                if (outcome.Success)
                {
                    _logger.Information ("Accepted Player Session: {PlayerSessionId}", playerSessionId);
                    return true;
                }

                _logger.Error ("ACCEPT PLAYER SESSION FAILED. AcceptPlayerSession() returned {Error}", outcome.Error);
                return false;
            } catch (Exception e)
            {
                _logger.Error (e, "ACCEPT PLAYER SESSION FAILED. AcceptPlayerSession() exception");
                return false;
            }
        }

        public bool RemovePlayerSession (string playerSessionId)
        {
            try
            {
                var outcome = GameLiftServerAPI.RemovePlayerSession (playerSessionId);
                if (outcome.Success)
                {
                    _logger.Information ("Removed Player Session: {PlayerSessionId}", playerSessionId);
                    return true;
                }

                _logger.Error ("REMOVE PLAYER SESSION FAILED. RemovePlayerSession() returned {Error}", outcome.Error);
                return false;
            } catch (Exception e)
            {
                _logger.Error (e, "REMOVE PLAYER SESSION FAILED. RemovePlayerSession() exception");
                return false;
            }
        }

        private void ProcessReady ()
        {
            try
            {
                var processParams = CreateProcessParameters ();
                var processReadyOutcome = GameLiftServerAPI.ProcessReady (processParams);

                if (processReadyOutcome.Success)
                {
                    _logger.Information ("PROCESSREADY SUCCESS");
                } else
                {
                    _logger.Error ("PROCESSREADY FAILED. ProcessReady() returned {Error}", processReadyOutcome.Error);
                }
            } catch (Exception e)
            {
                _logger.Error (e, "PROCESSREADY FAILED. ProcessReady() exception");
            }
        }

        private async UniTaskVoid ArmGameSessionCircuitBreaker ()
        {
            _logger.Information ("Arming game session circuit breaker...");
            await UniTask.Delay (TimeSpan.FromSeconds (_circuitBreakerTimeout));
            _logger.Information ("Game session termination because of circuit breaker");
            await TerminateGameSessionAsync ();
            _logger.Information ("Game session termination because of circuit breaker done");
        }

        private async void OnStartGameSession (Aws.GameLift.Server.Model.GameSession gameSession)
        {
            await UniTask.SwitchToMainThread ();

            try
            {
                _logger.Information ("GAMELIFT SESSION REQUESTED");

                var outcome = GameLiftServerAPI.ActivateGameSession ();
                if (outcome.Success)
                {
                    _logger.Information ("GAME SESSION ACTIVATED {GameSessionGameSessionId}", gameSession.GameSessionId);

                    var matchmakerData = gameSession.MatchmakerData;
                    var gameSessionMatchmakerData =
                        JsonConvert.DeserializeObject<GameSessionMatchmakerData> (matchmakerData);

                    GameMode gameMode;
                    List<string> playersIds;
                    string tournamentId;
                    string matchmakingConfigurationArn;

                    if (gameSessionMatchmakerData == null)
                    {
                        var localGameSessionData =
                            JsonConvert.DeserializeObject<LocalGameSessionData> (gameSession.GameSessionData);

                        if (localGameSessionData == null)
                            throw new InvalidOperationException (
                                "Cannot grab player ids not from MatchmakerData nor GameSessionData");

                        playersIds = localGameSessionData.PlayerIds;
                        gameMode = localGameSessionData.GameMode;
                        tournamentId = localGameSessionData.TournamentId;
                        matchmakingConfigurationArn = localGameSessionData.MatchmakingConfigurationArn;
                    } else
                    {
                        var team = gameSessionMatchmakerData.teams.FirstOrDefault ();
                        if (team == null)
                            throw new InvalidOperationException ("Malformed MatchmakerData");

                        playersIds = team
                            .players
                            .ConvertAll (x => x.playerId);

                        gameMode = playersIds.Count == 1 ? GameMode.PVE : GameMode.PVP;
                        tournamentId =
                            team
                                .players[0]
                                .attributes.FirstOrDefault (x => x.Key == "tournamentId")
                                .Value
                                ?.valueAttribute as string;
                        matchmakingConfigurationArn = gameSessionMatchmakerData.matchmakingConfigurationArn;

                        _logger.Information (
                            "matchmakerData {MatchmakerData} tournamentId {TournamentId} matchmakingConfigurationArn {MatchmakingConfigurationArn}",
                            matchmakerData,
                            tournamentId,
                            matchmakingConfigurationArn);
                    }

                    var preBattleService = _preBattleServiceFactory
                        .Create (gameMode)
                        .AddTo (_disposables);
                    preBattleService.Initialize ();

                    _logger.Information ("StartGameSession...");

                    _gameSessionProvider.GameSession = _gameSession = new GameSession
                    {
                        Id = gameSession.GameSessionId,
                        IpAddress = gameSession.IpAddress,
                        DnsName = gameSession.DnsName,
                        Port = gameSession.Port,
                        GameMode = gameMode,
                        TournamentId = tournamentId
                    };
                    var (players, level) = await _gameRepository.StartGameSession (
                        _gameSession,
                        playersIds,
                        matchmakingConfigurationArn);

                    _logger.Information ("StartGameSession completed");

                    _gameSession.Players = players;
                    _gameSession.Level = level;

                    ArmGameSessionCircuitBreaker ().Forget ();
                } else
                {
                    _logger.Error (
                        "GAME SESSION ACTIVATION FAILED. ActivateGameSession() returned {Error}",
                        outcome.Error);
                }
            } catch (Exception e)
            {
                _logger.Error (e, "GAME SESSION ACTIVATION FAILED. ActivateGameSession() exception");
            }
        }

        private async void OnUpdateGameSession (Aws.GameLift.Server.Model.UpdateGameSession _)
        {
            await UniTask.SwitchToMainThread ();

            _logger.Information ("GAMELIFT GAME SESSION UPDATE REQUESTED");
        }

        private async void OnProcessTerminate ()
        {
            await UniTask.SwitchToMainThread ();
            _logger.Information ("GAMELIFT PROCESS TERMINATION REQUESTED (OK BYE)");
            await TerminateGameSessionAsync ();
        }

        private ProcessParameters CreateProcessParameters ()
        {
            var logParameters = new LogParameters ();
            if (!string.IsNullOrEmpty (_serverProcessOptions.LogFile))
                logParameters.LogPaths = new List<string> { _serverProcessOptions.LogFile };

            return new ProcessParameters (
                onStartGameSession: OnStartGameSession,
                onUpdateGameSession: OnUpdateGameSession,
                onProcessTerminate: OnProcessTerminate,
                onHealthCheck: () => true,
                _serverProcessOptions.Port,
                logParameters);
        }

        public void Dispose ()
        {
            _disposables.Dispose ();
        }
    }
    #endif
}