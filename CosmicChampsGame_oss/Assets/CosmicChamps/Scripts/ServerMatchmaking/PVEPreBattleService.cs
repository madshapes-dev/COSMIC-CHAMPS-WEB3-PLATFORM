using System;
using System.Linq;
using CosmicChamps.Bootstrap.Server;
using CosmicChamps.Networking;
using CosmicChamps.Services;
using Cysharp.Threading.Tasks;
using Serilog;
using UniRx;
using UnityEngine.AddressableAssets;

namespace CosmicChamps.ServerMatchmaking
{
    #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
    public class PVEPreBattleService : IPreBattleService
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly IScenesLoadingService _scenesLoadingService;
        private readonly ServerNetworkService _networkService;
        private readonly HostClientNetworkService _hostClientNetworkService;
        private readonly IGameService _gameService;
        private readonly GameSessionProvider _gameSessionProvider;
        private readonly ILogger _logger;

        private bool _battleInProgress;

        public PVEPreBattleService (
            IScenesLoadingService scenesLoadingService,
            ServerNetworkService networkService,
            HostClientNetworkService hostClientNetworkService,
            IGameService gameService,
            GameSessionProvider gameSessionProvider,
            ILogger logger)
        {
            _scenesLoadingService = scenesLoadingService;
            _networkService = networkService;
            _hostClientNetworkService = hostClientNetworkService;
            _gameService = gameService;
            _gameSessionProvider = gameSessionProvider;
            _logger = logger;
        }

        private void OnClientAuthenticated (AuthData authData)
        {
            _logger.Information ("OnServerAuthenticated _battleInProgress {BattleInProgress}", _battleInProgress);

            if (_battleInProgress)
            {
                var gameSession = _gameSessionProvider.GameSession;
                _networkService.BattleStarting (authData, gameSession.Level, gameSession.GetCards ());

                return;
            }

            LoadLevel ().Forget ();
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

            while (string.IsNullOrEmpty (_gameSessionProvider.GameSession.Level))
            {
                await UniTask.NextFrame ();
            }

            var gameData = _gameService.GetCachedGameData ();
            var bot = _gameSessionProvider
                .GameSession
                .Players
                .FirstOrDefault (x => gameData.Bots.Any (y => y.PlayerId == x.Id));

            if (bot == null)
                throw new InvalidOperationException ("Cannot find bot player");

            _hostClientNetworkService.Start (bot.Id);

            await _scenesLoadingService.AppendScene (_gameSessionProvider.GameSession.Level, true);
            await _scenesLoadingService.AppendScene (Scenes.ServerBattle, false);

            _battleInProgress = true;
        }

        private void OnClientDisconnected (AuthData _)
        {
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