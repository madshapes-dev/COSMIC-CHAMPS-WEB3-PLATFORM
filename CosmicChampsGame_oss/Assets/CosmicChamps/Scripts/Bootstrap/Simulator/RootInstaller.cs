using CosmicChamps.Battle;
using CosmicChamps.Battle.PVE;
using CosmicChamps.Data.Sources.Tokens;
using CosmicChamps.Level;
using kcp2k;
using Mirror;
using Mirror.SimpleWeb;
using Oddworm.Framework;
using ThirdParty.Extensions;
using UnityEngine;
using Zenject;

namespace CosmicChamps.Bootstrap.Simulator
{
    public class RootInstaller : MonoInstaller
    {
        [SerializeField]
        private KcpTransport _transport;

        [SerializeField]
        private SimpleWebTransport _webGLTransport;

        [SerializeField]
        private MultiplexTransport _multiplexTransport;

        [SerializeField]
        private Authenticator _authenticator;

        [SerializeField]
        private LevelData _levelData;

        public override void InstallBindings ()
        {
            #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
            var token = CommandLine.GetString (CommandLineOptions.Token, string.Empty);

            Container.BindAsSingle<ITokensDataSource, SimulatorTokensDataSource> ().WithArguments (token);
            Container.BindInstance<Authenticator> (_authenticator);
            Container.BindInstance<KcpTransport> (_transport);
            Container.BindInstance<SimpleWebTransport> (_webGLTransport);
            Transport.active = _multiplexTransport;

            Container.BindInterfacesAndSelfTo<SimulatorNetworkService> ().AsSingle ();

            Container
                .BindInterfacesAndSelfTo<SimpleBotBehaviour> ()
                .AsSingle ()
                .NonLazy ();

            Container.Bind<LevelData> ().FromInstance (_levelData);

            DataLayerInstaller.Install (Container);
            #endif
        }
    }
}