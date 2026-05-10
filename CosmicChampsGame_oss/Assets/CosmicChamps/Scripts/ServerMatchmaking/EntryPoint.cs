using CosmicChamps.Data;
using CosmicChamps.Networking;
using CosmicChamps.Settings;
using UnityEngine;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.ServerMatchmaking
{
    public class EntryPoint : MonoBehaviour
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        [Inject]
        private GameLiftServer _gameLiftServer;

        [Inject]
        private ServerNetworkService _networkService;

        [Inject]
        private GameDataRepository _gameDataRepository;

        [Inject]
        private ServerProcessOptions _serverProcessOptions;

        [Inject]
        private ILogger _logger;

        private async void Start ()
        {
            _logger.Information ("LoadGameData...");
            await _gameDataRepository.LoadGameData ();

            _logger.Information ("Starting game lift server...");
            _gameLiftServer.Start (_serverProcessOptions);
            
            _logger.Information ("Starting network service...");
            _networkService.Start ((ushort)_serverProcessOptions.Port);
        }
        #endif
    }
}