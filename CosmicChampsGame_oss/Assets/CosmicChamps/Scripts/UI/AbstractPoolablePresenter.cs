using System;
using Zenject;

namespace CosmicChamps.UI
{
    public abstract class AbstractPoolablePresenter<TModel, TCallbacks, TSelf> : AbstractPresenter<TModel, TCallbacks, TSelf>,
        IPoolable<IMemoryPool>,
        IDisposable
        where TSelf : AbstractPoolablePresenter<TModel, TCallbacks, TSelf>
    {
        public abstract class AbstractFactory : PlaceholderFactory<TSelf>
        {
        }

        private IMemoryPool _pool;

        public void OnDespawned ()
        {
            Clear ();
            _pool = null;
        }

        public void OnSpawned (IMemoryPool pool)
        {
            _pool = pool;
        }

        public void Dispose ()
        {
            _pool?.Despawn (this);
        }
    }
}