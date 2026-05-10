using System;
using CosmicChamps.Battle.Data;
using CosmicChamps.Common;
using CosmicChamps.Networking;
using Cysharp.Threading.Tasks;
using Serilog;
using UniRx;
using Zenject;

namespace CosmicChamps.HomeScreen
{
    #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
    public class PreBattleService : IInitializable, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly ClientNetworkService _networkService;
        private readonly IScenesLoadingService _scenesLoadingService;
        private readonly ILogger _logger;

        public PreBattleService (
            ClientNetworkService networkService,
            IScenesLoadingService scenesLoadingService,
            ILogger logger)
        {
            _networkService = networkService;
            _scenesLoadingService = scenesLoadingService;
            _logger = logger;
        }

        public void Initialize ()
        {
            _networkService
                .BattleStarting
                .Subscribe (OnBattleStarting)
                .AddTo (_disposables);
        }

        private async UniTaskVoid OnBattleStartingAsync (BattleStartingData battleStartingData)
        {
            _logger.Information ("OnBattleStartingAsync ReplaceScene Level...");
            await _scenesLoadingService.ReplaceScene (
                battleStartingData.Level,
                true,
                "Starting the battle",
                false);
            _logger.Information ("OnBattleStartingAsync AppendScene ClientBattle...");
            await _scenesLoadingService.AppendScene (
                Scenes.ClientBattle,
                false,
                "Starting the battle",
                false,
                container => { container.Bind<BattleStartingData> ().FromInstance (battleStartingData); });
            _logger.Information ("OnBattleStartingAsync Done");
        }

        private void OnBattleStarting (BattleStartingData battleStartingData)
        {
            OnBattleStartingAsync (battleStartingData).Forget ();
        }

        public void Dispose ()
        {
            _disposables.Dispose ();
        }
    }
    #endif
}