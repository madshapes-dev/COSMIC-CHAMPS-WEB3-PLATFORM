using System;
using CosmicChamps.Battle.Data;
using UnityEngine;
using Zenject;

namespace CosmicChamps.Battle
{
    public abstract class AbstractUnitHPBar : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        private IMemoryPool _pool;

        public abstract void SetPlayerTeam (PlayerTeam playerTeam);
        public abstract void SetValue (UnitHp hp, bool immediate = false);
        public abstract void Align (Vector3 worldPosition);
        public abstract void SetLevel (int level);

        public void OnSpawned (IMemoryPool pool)
        {
            _pool = pool;
        }

        public void OnDespawned ()
        {
            _pool = null;
        }

        public void Dispose ()
        {
            _pool?.Despawn (this);
        }
    }
}