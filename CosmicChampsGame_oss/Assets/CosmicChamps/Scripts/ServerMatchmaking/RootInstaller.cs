using CosmicChamps.Battle;
using CosmicChamps.Networking;
using kcp2k;
using Mirror;
using Mirror.SimpleWeb;
using ThirdParty.Extensions;
using UnityEngine;
using Zenject;

namespace CosmicChamps.ServerMatchmaking
{
    public class RootInstaller : MonoInstaller
    {
        [SerializeField]
        private NetworkIdentity _playerPrefab;

        [SerializeField]
        private MultiplexTransport _multiplexTransport;

        [SerializeField]
        private SimpleWebTransport _simpleWebTransport;

        [SerializeField]
        private KcpTransport _transport;

        [SerializeField]
        private Authenticator _authenticator;

        [SerializeField]
        private float _gameSessionCircuitBreakerTimeout = 1f;

        public override void InstallBindings ()
        {
            #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
            Container.BindInstance<Authenticator> (_authenticator);
            Container.BindInstance<KcpTransport> (_transport);
            Container.BindInstance<SimpleWebTransport> (_simpleWebTransport);
            Transport.active = _multiplexTransport;

            Container
                .BindInterfacesAndSelfTo<ServerNetworkService> ()
                .AsSingle ()
                .WithArguments (new ServerNetworkService.Args (_playerPrefab, 2));

            Container
                .BindInterfacesAndSelfTo<HostClientNetworkService> ()
                .AsSingle ()
                .NonLazy ();

            Container.BindAsSingle<PreBattleServiceFactory> ();
            Container
                .BindAsSingle<GameLiftServer> ()
                .WithArguments (_gameSessionCircuitBreakerTimeout);
            #endif
        }
    }
}