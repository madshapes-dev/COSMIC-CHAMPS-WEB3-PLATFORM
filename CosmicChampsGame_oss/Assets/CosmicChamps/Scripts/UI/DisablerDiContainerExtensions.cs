using Zenject;

namespace CosmicChamps.UI
{
    public static class DisablerDiContainerExtensions
    {
        public static void AddDisabler (this DiContainer container) =>
            container.BindInterfacesTo<Disabler> ().AsSingle ().NonLazy ();
    }
}