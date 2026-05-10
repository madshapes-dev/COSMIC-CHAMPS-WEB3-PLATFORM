using CosmicChamps.Battle.PVE;
using CosmicChamps.Battle.Units;
using CosmicChamps.Bootstrap.Server;
using CosmicChamps.Common;
using CosmicChamps.Data;
using ThirdParty.Extensions;
using UnityEngine;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Battle.Server
{
    public class RootInstaller : MonoInstaller
    {
        [SerializeField]
        private float _battleSyncInterval;

        [SerializeField]
        private BaseUnitNetworkBehaviour _baseUnitPrefab;

        [SerializeField]
        private BaseUnitNetworkBehaviour _towerUnitPrefab;

        [Inject]
        private GameSessionProvider _gameSessionProvider;

        [Inject]
        private ILogger _logger;

        public override void InstallBindings ()
        {
            #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
            Container
                .BindInterfacesAndSelfTo<BattleService> ()
                .AsSingle ()
                .WithArguments (new BattleService.Args (_battleSyncInterval))
                .NonLazy ();

            Container.BindAsSingle<RegularUnitNetworkBehaviour.ServerFactory> ();
            Container
                .BindAsSingle<BaseUnitNetworkBehaviour.ServerFactory> ()
                .WithArguments (new BaseUnitNetworkBehaviour.FactoryArgs (_baseUnitPrefab, _towerUnitPrefab));

            Container
                .Bind (typeof (IUnitViewDataProvider))
                .To<AddressablesUnitViewDataProvider> ()
                .AsSingle ();

            Container
                .Bind<ITimeProvider> ()
                .To<UnityRealtimeSinceStartupTimeProvider> ()
                .AsSingle ();

            _logger.Information ("InstallBindings {GameMode}", _gameSessionProvider.GameSession.GameMode);
            if (_gameSessionProvider.GameSession.GameMode == GameMode.PVE)
                PVEInstaller.Install (Container);
            #endif
        }
    }
}