using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using CosmicChamps.Data;
using Mirror;
using UnityEngine;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class SpellTarget : ITarget
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private IUnit _unit;

        public ITarget Clone () => new SpellTarget
        {
            _unit = _unit
        };

        public bool IsDying => true;
        public string UnitId => _unit.Id;
        public UnitTargetType Type => UnitTargetType.Spell;
        public NetworkIdentity NetworkIdentity => _unit.NetworkIdentity;

        public (Vector3 world, Vector3 local) GetAttackPosition (IUnit attacker) => (Vector3.zero, Vector3.zero);

        public void ApplyDamage (int damage)
        {
        }

        public void OnStartServer (IUnit unit)
        {
            _unit = unit;
        }
        #endif
    }
}