using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using CosmicChamps.Data;
using Mirror;
using ThirdParty.Extensions;
using UnityEngine;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class UnitTarget : ITarget
    {
        [SerializeField]
        private Collider _collider;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private IUnit _unit;

        public bool IsDying => _unit.IsDying;
        public string UnitId => _unit.Id;

        public UnitTargetType Type => _unit.MovementType switch
        {
            UnitMovementType.Ground => UnitTargetType.Ground,
            UnitMovementType.Air => UnitTargetType.Air,
            UnitMovementType.Spell => UnitTargetType.Spell,
            _ => throw new ArgumentOutOfRangeException ()
        };

        public NetworkIdentity NetworkIdentity => _unit.NetworkIdentity;

        public (Vector3, Vector3) GetAttackPosition (IUnit attacker)
        {
            var worldPosition = _collider.Raycast (
                new Ray (attacker.Position.WithY (_collider.bounds.center.y), attacker.Forward),
                out var hit,
                float.MaxValue)
                //
                ? hit.point
                : _collider.bounds.center;

            return (worldPosition, _unit.InverseTransformPoint (worldPosition));
        }

        public void ApplyDamage (int damage) => _unit.ApplyDamage (damage);

        public ITarget Clone () => new UnitTarget
        {
            _collider = _collider
        };

        public void OnStartServer (IUnit unit)
        {
            _unit = unit;
        }
        #endif
    }
}