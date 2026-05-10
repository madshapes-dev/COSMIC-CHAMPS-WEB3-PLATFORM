using Zenject;

namespace CosmicChamps.Battle.PVE
{
    #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
    public class PVEInstaller : Installer<PVEInstaller>
    {
        public override void InstallBindings ()
        {
            Container
                .BindInterfacesAndSelfTo<SimpleBotBehaviour> ()
                .AsSingle ()
                .NonLazy ();
        }
    }
    #endif
}