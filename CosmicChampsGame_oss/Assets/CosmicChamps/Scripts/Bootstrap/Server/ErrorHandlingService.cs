using System;
using System.Globalization;
using CosmicChamps.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;


namespace CosmicChamps.Bootstrap.Server
{
    #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
    public class ErrorHandlingService : IInitializable, IDisposable
    {
        private readonly GameRepository _gameRepository;
        private readonly GameSessionProvider _gameSessionProvider;
        private readonly Serilog.ILogger _logger;

        public ErrorHandlingService (
            GameRepository gameRepository,
            GameSessionProvider gameSessionProvider,
            Serilog.ILogger logger)
        {
            _gameRepository = gameRepository;
            _gameSessionProvider = gameSessionProvider;
            _logger = logger;
        }

        private async UniTaskVoid ReportErrorAsync (string message, string stacktrace)
        {
            var gameSession = _gameSessionProvider.GameSession;
            var gameSessionReport = gameSession != null
                ? $"Game Session: {gameSession.Id}\nServer Address: {gameSession.DnsName}:{gameSession.Port}"
                : string.Empty;

            var reportId = await _gameRepository.ReportError (
                BuildInfo.AppVersionString,
                $"{Application.platform}(Server)",
                DateTime.Now.ToString (CultureInfo.InvariantCulture),
                gameSessionReport,
                message,
                stacktrace);

            _logger.Information ("Error reported with id {ReportId}", reportId);
        }

        private void OnLogMessageReceived (string condition, string stacktrace, LogType type)
        {
            if (type != LogType.Exception)
                return;

            ReportError (condition, stacktrace);
        }

        public void ReportError (string message, string stacktrace) => ReportErrorAsync (message, stacktrace).Forget ();

        public void ReportError (Exception e) => ReportError (e.Message, e.StackTrace);

        public void Initialize ()
        {
            Application.logMessageReceived += OnLogMessageReceived;
        }

        public void Dispose ()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }
    }
    #endif
}