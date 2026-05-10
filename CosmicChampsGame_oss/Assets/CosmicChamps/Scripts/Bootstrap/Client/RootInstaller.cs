using Algorand.Unity.WalletConnect;
using CosmicChamps.Battle;
using CosmicChamps.Bootstrap.Client.UI;
using CosmicChamps.Common;
using CosmicChamps.HomeScreen;
using CosmicChamps.Networking;
using CosmicChamps.Services;
using CosmicChamps.Settings;
using CosmicChamps.UI;
using CosmicChamps.Utils;
using kcp2k;
using Mirror;
using Mirror.SimpleWeb;
using ThirdParty.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CosmicChamps.Bootstrap.Client
{
    public class RootInstaller : MonoInstaller
    {
        [Header ("UI"), SerializeField]
        private SplashScreenPresenter _splashScreenPresenter;

        [SerializeField]
        private ErrorPresenter _errorPresenter;

        [SerializeField]
        private MaintenancePopup _maintenancePopup;

        [SerializeField]
        private UpdateRequiredPopup _updateRequiredPopup;

        [SerializeField]
        private EventSystem _eventSystem;

        [Header ("Mirror Networking"), SerializeField]
        private KcpTransport _transport;

        [SerializeField]
        private SimpleWebTransport _webGLTransport;

        [SerializeField]
        private MultiplexTransport _multiplexTransport;

        [SerializeField]
        private NetworkIdentity _playerPrefab;

        [SerializeField]
        private Authenticator _authenticator;

        [Header ("Grpc/HTTP Networking"), SerializeField]
        private NetworkingConfig _networkingConfig;

        [Header ("Scene loading delays"), SerializeField]
        private float _beforeSceneLoadDelay = 0.25f;

        [SerializeField]
        private float _afterSceneLoadDelay = 0.5f;

        [Header ("Sounds"), SerializeField]
        private SoundsService _soundsService;

        [Header ("Algo Wallet SDK"), SerializeField]
        private ClientMeta _algoClientMeta;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        public override void InstallBindings ()
        {
            Container.Bind<AddressablesZenjectSceneLoader> ().AsSingle ();

            Container.BindAsSingle<IPresenterActivator, CanvasGroupFaderPresenterActivator> ();
            Container.BindAsSingle<IScenesLoadingService, ClientScenesLoadingService> ();
            Container
                .BindAsSingle<ScenesManager> ()
                .WithArguments (new ScenesManager.Args (_beforeSceneLoadDelay, _afterSceneLoadDelay));
            Container.BindInstance<SplashScreenPresenter> (_splashScreenPresenter);
            Container.BindInstance<ErrorPresenter> (_errorPresenter);
            Container.BindInstance<MaintenancePopup> (_maintenancePopup);
            Container.BindInstance<UpdateRequiredPopup> (_updateRequiredPopup);
            Container.BindInterfacesAndSelfTo<ErrorHandlingService> ().AsSingle ().NonLazy ();
            Container.BindInterfacesAndSelfTo<UpdateHandlingService> ().AsSingle ().NonLazy ();
            Container.BindInstance<EventSystem> (_eventSystem);
            Container.BindAsSingle<UILocker> ();

            Container
                .BindInterfacesAndSelfTo<ClientNetworkService> ()
                .AsSingle ()
                .WithArguments (new ClientNetworkService.Args (_playerPrefab));

            Container
                .BindInterfacesTo<PreBattleService> ()
                .AsSingle ()
                .NonLazy ();

            Container.BindInstance<Authenticator> (_authenticator);
            Container.BindInstance<KcpTransport> (_transport);
            Container.BindInstance<SimpleWebTransport> (_webGLTransport);
            Transport.active = _multiplexTransport;

            Container.BindInstance<NetworkingConfig> (_networkingConfig);
            Container.BindInstance<SoundsService> (_soundsService);

            Container
                .Bind (typeof (ICardViewDataProvider), typeof (IRestartListener))
                .To<AddressablesCardViewDataProvider> ()
                .AsSingle ();

            Container
                .Bind (typeof (IUnitViewDataProvider), typeof (IRestartListener))
                .To<AddressablesUnitViewDataProvider> ()
                .AsSingle ();

            Container
                .Bind (typeof (IEmojisProvider), typeof (IRestartListener))
                .To<AddressablesEmojisProvider> ()
                .AsSingle ();

            Container
                .Bind (typeof (IShardsViewProvider), typeof (IRestartListener))
                .To<AddressablesShardsViewProvider> ()
                .AsSingle ();

            Container
                .Bind (typeof (IBrandingFactory), typeof (IRestartListener))
                .To<AddressablesBrandingFactory> ()
                .AsSingle ();

            Container
                .Bind (typeof (IWalletBridgeProvider), typeof (MutableWalletBridgeProvider))
                .To<MutableWalletBridgeProvider> ()
                .AsSingle ();

            Container.AddDisabler ();
            Container.BindFactory<NetworkReachableReporterProgress, NetworkReachableReporterProgress.Factory> ();

            Container
                .BindInterfacesAndSelfTo<AdService> ()
                .AsSingle ()
                .NonLazy ();

            
            Container
                .Bind<IImmutableService> ()
                #if UNITY_SERVER
                .To<DummyImmutableService>()
                #else
                .To<ImmutableService>()
                #endif
                .AsSingle ();

            DataLayerInstaller.Install (Container, _networkingConfig);
        }
        #endif
    }
}