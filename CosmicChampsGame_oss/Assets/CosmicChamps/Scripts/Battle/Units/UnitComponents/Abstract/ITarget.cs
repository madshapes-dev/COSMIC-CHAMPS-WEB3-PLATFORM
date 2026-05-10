using CosmicChamps.Data;
using Mirror;
using UnityEngine;

namespace CosmicChamps.Battle.Units.UnitComponents.Abstract
{
    public interface ITarget : IUnitComponent<ITarget>
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        bool IsDying { get; }
        string UnitId { get; }
        UnitTargetType Type { get; }
        NetworkIdentity NetworkIdentity { get; }

        (Vector3 world, Vector3 local) GetAttackPosition (IUnit attacker);
        void ApplyDamage (int damage);
        #endif
    }
}