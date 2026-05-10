using System;
using System.Threading;
using CosmicChamps.Battle.Data;
using CosmicChamps.Data;
using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;

namespace CosmicChamps.Battle.Units.UnitComponents.Abstract
{
    public interface IUnit
    {
        string Id { get; }

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        UnitHp Hp { get; }
        PlayerTeam Team { get; }
        IUnitStats Stats { get; }
        int Level { get; }
        UnitViewParams ViewParams { get; }
        UnitMovementType MovementType { get; }
        bool IsDying { get; }
        IObservable<IUnit> OnDying { get; }
        Vector3 Position { get; }
        Vector3 Forward { get; }
        NetworkIdentity NetworkIdentity { get; }

        T GetUnitComponent<T> (string id = IUnitComponent.NoId) where T : IUnitComponent;
        void ApplyDamage (int damage);
        UniTask AimTarget (ITarget target, float duration, CancellationToken cancellationToken);
        void StopBattle ();
        Vector3 InverseTransformPoint (Vector3 worldPoint);
        #endif

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        #endif
    }
}