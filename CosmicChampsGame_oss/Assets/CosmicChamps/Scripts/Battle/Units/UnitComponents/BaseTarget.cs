using System;
using System.Collections.Generic;
using CosmicChamps.Battle.Data;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using CosmicChamps.Data;
using Mirror;
using ThirdParty.Extensions;
using UniRx;
using UnityEngine;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class BaseTarget : ITarget
    {
        [SerializeField]
        private Collider _collider;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private readonly CompositeDisposable _disposables = new();
        private readonly Dictionary<IUnit, (Vector3, Vector3)> _positions = new();

        private IUnit _unit;

        public bool IsDying => _unit.IsDying;
        public string UnitId => _unit.Id;
        public UnitTargetType Type => UnitTargetType.Base;
        public NetworkIdentity NetworkIdentity => _unit.NetworkIdentity;

        public (Vector3, Vector3) GetAttackPosition (IUnit attacker)
        {
            if (_positions.TryGetValue (attacker, out var position))
                return position;

            var bounds = _collider.bounds;
            // bounds.Draw (_unit.Team == PlayerTeam.North ? Color.blue : Color.red, 60f);
            var worldPosition = bounds.GetRandomPosition ();
            worldPosition = _unit.Team == PlayerTeam.North
                ? worldPosition.WithZ (bounds.max.z)
                : worldPosition.WithZ (bounds.min.z);

            position = (worldPosition, _unit.InverseTransformPoint (worldPosition));
            _positions.Add (attacker, position);

            attacker
                .OnDying
                .Subscribe (_ => _positions.Remove (attacker))
                .AddTo (_disposables);

            return position;
        }

        public void ApplyDamage (int damage) => _unit.ApplyDamage (damage);

        public void OnStartServer (IUnit unit)
        {
            _unit = unit;
        }

        public void Dispose () => _disposables.Dispose ();

        public ITarget Clone () => new BaseTarget
        {
            _collider = _collider
        };
        #endif
    }
}