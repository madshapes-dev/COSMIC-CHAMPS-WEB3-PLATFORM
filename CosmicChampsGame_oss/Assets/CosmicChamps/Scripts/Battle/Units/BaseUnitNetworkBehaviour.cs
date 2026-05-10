using System;
using System.Linq;
using System.Threading;
using CosmicChamps.Battle.Client;
using CosmicChamps.Battle.Data;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using CosmicChamps.Battle.Units.UnitComponents.Setups;
using CosmicChamps.Data;
using CosmicChamps.Level;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mirror;
using ThirdParty.Extensions;
using UnityEngine;
using Zenject;

namespace CosmicChamps.Battle.Units
{
    public class BaseUnitNetworkBehaviour : AbstractUnitNetworkBehaviour
    {
        public readonly struct FactoryArgs
        {
            public readonly BaseUnitNetworkBehaviour BasePrefab;
            public readonly BaseUnitNetworkBehaviour TowerPrefab;

            public FactoryArgs (BaseUnitNetworkBehaviour basePrefab, BaseUnitNetworkBehaviour towerPrefab)
            {
                BasePrefab = basePrefab;
                TowerPrefab = towerPrefab;
            }
        }

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        public class ServerFactory
        {
            private readonly DiContainer _container;
            private readonly LevelData _levelData;
            private readonly FactoryArgs _args;

            public ServerFactory (DiContainer container, LevelData levelData, FactoryArgs args)
            {
                _container = container;
                _args = args;
                _levelData = levelData;
            }

            private void AddUnitComponent<T> (
                    BaseUnitNetworkBehaviour instance,
                    UnitComponentSetup<T> unitComponentSetup,
                    string id = IUnitComponent.NoId)
                //
                where T : IUnitComponent<T>
            {
                var prototype = unitComponentSetup.Prototype;
                if (prototype == null)
                    return;

                var component = prototype.Clone ();
                _container.Inject (component);
                instance.AddUnitComponent (component, id);
            }

            public BaseUnitNetworkBehaviour Create (
                string playerId,
                Transform placeholder,
                PlayerTeam playerTeam,
                BaseUnitType type,
                UnitData unitData,
                int level)
            {
                var prefab = type switch
                {
                    BaseUnitType.Base => _args.BasePrefab,
                    BaseUnitType.Turret => _args.TowerPrefab,
                    BaseUnitType.Shield => _args.TowerPrefab,
                    _ => throw new ArgumentOutOfRangeException (nameof (type), type, null)
                };
                if (prefab == null)
                    throw new Exception ($"Prefab for {type} not found");

                var instance = _container.InstantiatePrefabForComponent<BaseUnitNetworkBehaviour> (prefab);
                var instanceTransform = instance.transform;
                instanceTransform.position = placeholder.position;
                instanceTransform.rotation = placeholder.rotation;
                instanceTransform.localScale = placeholder.localScale;

                instance._ownerId = playerId;
                instance._type = type;
                instance._unitData = unitData;
                instance._level = level;
                instance._cannon = _levelData
                    .GetBase (playerTeam)
                    .Cannons
                    .FirstOrDefault (x => x.Type == type);
                instance._unitId = instance._unitData.Id;
                instance.SetTeam (playerTeam);

                AddUnitComponent (instance, instance._animatorSetup);
                AddUnitComponent (instance, instance._attackSetup);
                AddUnitComponent (instance, instance._damagerSetup, IDamager.Default);
                AddUnitComponent (instance, instance._deathSetup);
                AddUnitComponent (instance, instance._deathDamagerSetup, IDamager.Death);
                AddUnitComponent (instance, instance._movementSetup);
                AddUnitComponent (instance, instance._targetSetup);
                AddUnitComponent (instance, instance._targetSeekerSetup);

                return instance;
            }
        }
        #endif

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        public class ClientFactory
        {
            private readonly DiContainer _container;
            private readonly FactoryArgs _args;

            public ClientFactory (DiContainer container, FactoryArgs args)
            {
                _container = container;
                _args = args;
            }

            private void RegisterUnitPrefab<TPrefab> (DiContainer container, TPrefab prefab)
                where TPrefab : Component
            {
                NetworkClient.RegisterPrefab (
                    prefab.gameObject,
                    msg =>
                    {
                        var instance = container.InstantiatePrefabForComponent<BaseUnitNetworkBehaviour> (prefab);
                        var instanceTransform = instance.transform;
                        instanceTransform.localPosition = msg.position;
                        instanceTransform.localRotation = msg.rotation;
                        instanceTransform.localScale = msg.scale;
                        instance.name = $"{prefab.name} [connId={msg.netId}]";

                        return instance.gameObject;
                    },
                    Destroy);
            }

            public void Initialize ()
            {
                RegisterUnitPrefab (_container, _args.BasePrefab);
                RegisterUnitPrefab (_container, _args.TowerPrefab);
            }
        }
        #endif

        [SyncVar]
        private string _ownerId;

        [SyncVar]
        private BaseUnitType _type;

        private LevelData.Cannon _cannon;
        private ShipFlyController _shipFlyController;

        public BaseUnitType Type => _type;

        [ClientRpc]
        private void ClientRotateTurret (Quaternion quaternion, float duration)
        {
            if (_cannon == null)
                throw new InvalidOperationException (
                    $"Unable to find cannon for playerTeam: {Team}; type: {_type}");

            _cannon
                .Pivot
                .DORotateQuaternion (quaternion, duration);
        }

        [ClientRpc]
        private void ClientExplode ()
        {
            if (_shipFlyController != null)
                _shipFlyController.Explode (_type);
        }

        public string OwnerId => _ownerId;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        public override IUnitStats Stats => _unitData.Stats;

        public override async UniTask AimTarget (ITarget target, float duration, CancellationToken cancellationToken)
        {
            var attackPosition = target.GetAttackPosition (this);

            _target = target.NetworkIdentity;
            _targetWorldAimPosition = attackPosition.world;
            _targetLocalAttackPosition = attackPosition.local;

            var cannonPivot = _cannon.Pivot;
            var turretPosition = cannonPivot.position;
            var targetRotation =
                Quaternion.LookRotation (attackPosition.world.WithY (turretPosition.y) - turretPosition);
            var rotationDuration = Mathf.Abs (Quaternion.Angle (cannonPivot.rotation, targetRotation)) /
                                   ViewParams.TurnTargetDuration;

            cannonPivot.rotation = targetRotation;
            ClientRotateTurret (targetRotation, rotationDuration);

            await UniTask
                .Delay (TimeSpan.FromSeconds (rotationDuration), cancellationToken: cancellationToken)
                .SuppressCancellationThrow ();
        }

        public override void StopBattle ()
        {
            StopLifeLoop ();
        }

        protected override void Die ()
        {
            base.Die ();
            ClientExplode ();
        }
        #endif

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        /**
         * Using InjectOptional to prevent Zenject exceptions when running server in the editor without UNITY_EDITOR_MIRROR_STRIP defined.
         * The code which is related to the client is not stripped in the editor mode without UNITY_EDITOR_MIRROR_STRIP. But it is not used
         * so missed dependencies do not produce any errors.
         */
        [InjectOptional]
        private BattleService _battleService;

        [InjectOptional]
        private LevelData _levelData;

        private BaseUnitHPBar _hpBar;

        public override void OnStartClient ()
        {
            if (NetworkClient.isHostClient)
                return;

            var isPlayer = _ownerId == _battleService.Player.Id;

            _cannon = _levelData
                .GetBase (Team)
                .Cannons
                .FirstOrDefault (x => x.Type == _type);

            _shipFlyController = _levelData
                .GetBase (Team)
                .ShipFlyController;

            name = $"{name} {(isPlayer ? "Player" : "Opponent")} {_type} {Team}";

            FireOnStartClient ();
        }
        #endif
    }
}