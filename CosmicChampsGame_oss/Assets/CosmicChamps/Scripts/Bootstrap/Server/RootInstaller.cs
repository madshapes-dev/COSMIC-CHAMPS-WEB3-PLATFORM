using ThirdParty.Extensions;
using Zenject;

namespace CosmicChamps.Bootstrap.Server
{
    public class RootInstaller : MonoInstaller
    {
        public override void InstallBindings ()
        {
            #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
            Container.Bind<AddressablesZenjectSceneLoader> ().AsSingle ();

            Container
                .BindAsSingle<ScenesManager> ()
                .WithArguments (new ScenesManager.Args (0.1f, 0.1f));

            Container.BindAsSingle<IScenesLoadingService, ServerScenesLoadingService> ();
            Container.BindInterfacesAndSelfTo<ErrorHandlingService> ().AsSingle ().NonLazy ();
            Container.Bind<GameSessionProvider> ().AsSingle ();
            Container.Bind<PlayersProvider> ().AsSingle ();

            DataLayerInstaller.Install (Container);
            #endif
        }
    }
}