using System;
using System.Collections.Generic;
using CosmicChamps.Bootstrap.Server;
using CosmicChamps.Networking;
using Cysharp.Threading.Tasks;
using Serilog;
using UniRx;
using UnityEngine.AddressableAssets;

namespace CosmicChamps.ServerMatchmaking
{
    #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
    public class PVPPreBattleService : IPreBattleService
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly List<AuthData> _authDatas = new();

        private readonly IScenesLoadingService _scenesLoadingService;
        private readonly ServerNetworkService _networkService;
        private readonly ILogger _logger;
        private readonly GameSessionProvider _gameSessionProvider;

        private bool _battleInProgress;

        public PVPPreBattleService (
            IScenesLoadingService scenesLoadingService,
            ServerNetworkService networkService,
            ILogger logger,
            GameSessionProvider gameSessionProvider)
        {
            _scenesLoadingService = scenesLoadingService;
            _networkService = networkService;
            _logger = logger;
            _gameSessionProvider = gameSessionProvider;
        }

        private void OnClientAuthenticated (AuthData authData)
        {
            _authDatas.Add (authData);
            _logger.Information ("OnServerAuthenticated authenticatedClientsCount {AuthDatasCount}", _authDatas.Count);

            switch (_authDatas.Count)
            {
                case 1:
                    break;
                case 2:
                    if (!_battleInProgress)
                    {
                        LoadLevel ().Forget ();
                    } else
                    {
                        var gameSession = _gameSessionProvider.GameSession;
                        _networkService.BattleStarting (authData, gameSession.Level, gameSession.GetCards ());
                    }

                    break;
                default:
                    throw new InvalidOperationException ("Wtf?");
            }
        }

        private async UniTaskVoid LoadLevel ()
        {
            _logger.Information ("CheckForCatalog...");
            var catalogIds = await Addressables.CheckForCatalogUpdates ();
            _logger.Information ("CheckForCatalog Done {Join}", string.Join (", ", catalogIds));
            if (catalogIds.Count > 0)
            {
                _logger.Information ("UpdateCatalogs...");
                await Addressables.UpdateCatalogs (true, catalogIds);
                _logger.Information ("UpdateCatalogs Done");
            }

            _logger.Information ("GetDownloadSize...");
            var getDownloadSize = await Addressables.GetDownloadSizeAsync (AddressablesLabels.Server);
            _logger.Information ("GetDownloadSize Done {DownloadSize}", getDownloadSize);
            if (getDownloadSize > 0)
            {
                _logger.Information ("DownloadDependencies...");
                await Addressables.DownloadDependenciesAsync (AddressablesLabels.Server, true);
                _logger.Information ("DownloadDependencies Done");
            }

            _logger.Information ("Waiting for _gameSessionProvider.GameSession.Level...");
            while (string.IsNullOrEmpty (_gameSessionProvider.GameSession.Level))
            {
                await UniTask.NextFrame ();
            }

            _logger.Information ("_gameSessionProvider.GameSession.Level Done {Level}", _gameSessionProvider.GameSession.Level);

            await _scenesLoadingService.AppendScene (_gameSessionProvider.GameSession.Level, true);
            await _scenesLoadingService.AppendScene (Scenes.ServerBattle, false);

            _battleInProgress = true;
        }

        private void OnClientDisconnected (AuthData authData)
        {
            _authDatas.Remove (authData);
        }

        public void Initialize ()
        {
            _networkService.ClientAuthenticated.Subscribe (OnClientAuthenticated).AddTo (_disposables);
            _networkService.ClientDisconnected.Subscribe (OnClientDisconnected).AddTo (_disposables);
        }

        public void Dispose ()
        {
            _disposables.Dispose ();
        }
    }
    #endif
}