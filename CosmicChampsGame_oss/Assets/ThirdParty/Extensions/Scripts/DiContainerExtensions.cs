using UnityEngine;
using Zenject;

namespace ThirdParty.Extensions
{
    public static class DiContainerExtensions
    {
        public static ArgConditionCopyNonLazyBinder BindPoolablePresenterPool<TPresenter, TFactory> (
                this DiContainer diContainer,
                TPresenter prefab,
                Transform parent = null)
            //
            where TPresenter : Component, IPoolable<IMemoryPool>
            where TFactory : PlaceholderFactory<TPresenter>
        {
            if (parent == null)
                parent = new GameObject (prefab.name).transform;

            return diContainer
                .BindFactory<TPresenter, TFactory> ()
                .FromMonoPoolableMemoryPool (
                    x => x
                        .FromComponentInNewPrefab (prefab)
                        .UnderTransform (parent));
        }

        public static ConcreteIdArgConditionCopyNonLazyBinder BindAsSingle<TContract> (this DiContainer container) =>
            container
                .Bind<TContract> ()
                .AsSingle ();

        public static ConcreteIdArgConditionCopyNonLazyBinder BindAsSingle<TContract, TConcrete> (this DiContainer container)
            where TConcrete : TContract =>
            container
                .Bind<TContract> ()
                .To<TConcrete> ()
                .AsSingle ();

        public static IfNotBoundBinder BindAsSingleNonLazy<TContract> (this DiContainer container) =>
            container
                .Bind<TContract> ()
                .AsSingle ()
                .NonLazy ();
    }
}