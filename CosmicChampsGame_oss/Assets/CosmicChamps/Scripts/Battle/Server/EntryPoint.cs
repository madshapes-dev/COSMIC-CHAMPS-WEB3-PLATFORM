using CosmicChamps.Bootstrap.Server;
using CosmicChamps.Common;
using CosmicChamps.Data;
using CosmicChamps.Networking;
using UnityEngine;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Battle.Server
{
    public class EntryPoint : MonoBehaviour
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        [Inject]
        private ServerNetworkService _serverNetworkService;

        [Inject]
        private GameDataRepository _gameDataRepository;

        [Inject]
        private GameSessionProvider _gameSessionProvider;

        [Inject]
        private IUnitViewDataProvider _unitViewDataProvider;

        [Inject]
        private ILogger _logger;

        private async void Start ()
        {
            var gameSession = _gameSessionProvider.GameSession;

            _logger.Information ("Prewarm...");
            await _unitViewDataProvider.Prewarm (gameSession.GetUnitIds ());

            _logger.Information ("LoadGameData...");
            await _gameDataRepository.LoadGameData ();

            _logger.Information ("BattleStarting...");
            _serverNetworkService.BattleStarting (gameSession.Level, gameSession.GetCards ());
        }
        #endif
    }
}