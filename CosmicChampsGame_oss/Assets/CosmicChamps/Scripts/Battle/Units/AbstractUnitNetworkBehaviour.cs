using System;
using System.Collections.Generic;
using System.Threading;
using CosmicChamps.Battle.Data;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using CosmicChamps.Battle.Units.UnitComponents.Setups;
using CosmicChamps.Data;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mirror;
using Pathfinding;
using ThirdParty.Extensions;
using ThirdParty.Extensions.Components;
using UnityEngine;

namespace CosmicChamps.Battle.Units
{
    public abstract class AbstractUnitNetworkBehaviour : NetworkBehaviour, IUnit
    {
        [Header ("Components"), SerializeField]
        protected AnimatorSetup _animatorSetup;

        [SerializeField]
        protected AttackSetup _attackSetup;

        [SerializeField]
        protected DamagerSetup _damagerSetup;

        [SerializeField]
        protected DeathSetup _deathSetup;

        [SerializeField]
        protected DamagerSetup _deathDamagerSetup;

        [SerializeField]
        protected MovementSetup _movementSetup;

        [SerializeField]
        protected TargetSetup _targetSetup;

        [SerializeField]
        protected TargetSeekerSetup _targetSeekerSetup;

        [SyncVar]
        protected string _unitId;

        [SyncVar]
        private UnitHp _hp;

        [SyncVar]
        private PlayerTeam _team;
        
        [SyncVar]
        protected int _level;

        [SyncVar]
        protected NetworkIdentity _target;

        [SyncVar]
        protected Vector3 _targetLocalAttackPosition;

        [SyncVar]
        protected Vector3 _targetWorldAimPosition;
        
        public UnitHp Hp => _hp;
        public PlayerTeam Team => _team;
        public string Id => _unitId;
        public int Level => _level;

        public Vector3 TargetPosition =>
            _target != null
                ? _target.transform.TransformPoint (_targetLocalAttackPosition)
                : _targetWorldAimPosition;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private readonly struct ComponentKey
        {
            private readonly string _id;
            private readonly Type _type;

            public ComponentKey (string id, Type type)
            {
                _id = id;
                _type = type;
            }

            private bool Equals (ComponentKey other)
            {
                return _id == other._id && _type == other._type;
            }

            public override bool Equals (object obj)
            {
                if (ReferenceEquals (null, obj)) return false;
                if (ReferenceEquals (this, obj)) return true;
                if (obj.GetType () != GetType ()) return false;
                return Equals ((ComponentKey)obj);
            }

            public override int GetHashCode ()
            {
                return HashCode.Combine (_id, _type);
            }
        }

        private readonly ObservableEvent<IUnit> _onDying = new();
        private readonly ObservableEvent<IUnit> _onStartServer = new();
        private readonly CancellationTokenSource _lifeLoopCancellationTokenSource = new();
        private readonly Dictionary<ComponentKey, IUnitComponent> _unitComponents = new();

        protected UnitData _unitData;

        public IObservable<IUnit> OnDying => _onDying.AsObservable ();
        public IObservable<IUnit> onStartServer => _onStartServer.AsObservable ();
        public Vector3 Position => transform.position;
        public Vector3 Forward => transform.forward;
        public NetworkIdentity NetworkIdentity => netIdentity;

        public abstract IUnitStats Stats { get; }
        public UnitViewParams ViewParams => _unitData.ViewParams;
        public UnitMovementType MovementType => _unitData.MovementType;

        public void AddUnitComponent<T> (string id = IUnitComponent.NoId) where T : IUnitComponent, new () =>
            _unitComponents.Add (new ComponentKey (id, typeof (T)), new T ());

        public void AddUnitComponent<T> (T component, string id = IUnitComponent.NoId) where T : IUnitComponent =>
            _unitComponents.Add (new ComponentKey (id, typeof (T)), component);

        public T GetUnitComponent<T> (string id = IUnitComponent.NoId) where T : IUnitComponent =>
            _unitComponents.TryGetValue (new ComponentKey (id, typeof (T)), out var component)
                ? (T)component
                : default;

        public bool IsDying { private set; get; }

        private void SetHp (int hp)
        {
            _hp = new UnitHp
            {
                Value = hp,
                NormalizedValue = Mathf.Clamp01 ((float)hp / Stats.Hp[_level])
            };
        }

        private async UniTaskVoid LifeLoop (CancellationToken cancellationToken)
        {
            var totalCancellationToken = this.GetCancellationTokenOnDestroy ();

            try
            {
                var deployDuration = _unitData.ViewParams?.DeployDuration ?? 0;
                if (deployDuration > 0)
                {
                    await UniTask.NextFrame (totalCancellationToken);
                    GetUnitComponent<IAnimator> ().Deploy ();
                    await UniTask.Delay (
                        TimeSpan.FromSeconds (deployDuration),
                        cancellationToken: cancellationToken);
                }

                var movement = GetUnitComponent<IMovement> ();
                var attack = GetUnitComponent<IAttack> ();

                while (true)
                {
                    if (Stats.Damage != null)
                    {
                        await movement.ApproachTarget (cancellationToken);
                        await attack.PerformAttack (cancellationToken, totalCancellationToken);
                    }

                    await UniTask.Yield (cancellationToken);
                }
            } catch (OperationCanceledException)
            {
                if (totalCancellationToken.IsCancellationRequested)
                    return;

                if (IsDying)
                {
                    var death = GetUnitComponent<IDeath> ();
                    await death
                        .Die (totalCancellationToken)
                        .SuppressCancellationThrow ();

                    if (!totalCancellationToken.IsCancellationRequested)
                        NetworkServer.Destroy (gameObject);
                }
            }
        }

        protected void StopLifeLoop ()
        {
            _lifeLoopCancellationTokenSource.Cancel ();
        }

        protected void FireOnStartServer () => _onStartServer.Fire (this);

        protected virtual void Die ()
        {
            IsDying = true;
            StopLifeLoop ();

            _onDying.Fire (this);
        }

        public void SetTeam (PlayerTeam team)
        {
            _team = team;
        }

        public void ApplyDamage (int damage)
        {
            if (IsDying)
                return;
            
            SetHp (Mathf.Clamp (_hp.Value - damage, 0, int.MaxValue));
            if (_hp.Value > 0)
                return;

            Die ();
        }

        public virtual async UniTask AimTarget (ITarget target, float duration, CancellationToken cancellationToken)
        {
            var attackPosition = target.GetAttackPosition (this);

            _target = target.NetworkIdentity;
            _targetWorldAimPosition = attackPosition.world;
            _targetLocalAttackPosition = attackPosition.local;

            var position = transform.position;
            var targetRotation = Quaternion.LookRotation (attackPosition.world.WithY (position.y) - position);
            
            await transform.DORotateQuaternion (targetRotation, duration);
            await UniTask.Delay (
                TimeSpan.FromSeconds (duration),
                cancellationToken: cancellationToken);
        }

        public virtual void StopBattle ()
        {
            StopLifeLoop ();
            GetUnitComponent<IMovement> ().Stop ();
        }

        public Vector3 InverseTransformPoint (Vector3 worldPoint) => transform.InverseTransformPoint (worldPoint);

        public override void OnStartServer ()
        {
            base.OnStartServer ();

            SetHp (Stats.Hp[_level]);

            foreach (var unitComponent in _unitComponents.Values)
            {
                unitComponent.OnStartServer (this);
            }

            GetUnitComponent<IAnimator> ().SetMovementSpeed (ViewParams?.MoveAnimationSpeed ?? 1);

            LifeLoop (_lifeLoopCancellationTokenSource.Token).Forget ();

            FireOnStartServer ();
        }

        public override void OnStopServer ()
        {
            base.OnStopServer ();

            StopLifeLoop ();
            foreach (var unitComponent in _unitComponents.Values)
            {
                unitComponent.OnStopServer ();
                unitComponent.Dispose ();
            }

            _unitComponents.Clear ();
        }
        #endif

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        private readonly ObservableEvent<IUnit> _onStartClient = new();

        public IObservable<IUnit> onStartClient => _onStartClient.AsObservable ();

        protected void FireOnStartClient () => _onStartClient.Fire (this);

        public override void OnStartClient ()
        {
            if (NetworkClient.isHostClient)
                return;

            var seeker = GetComponent<Seeker> ();
            if (seeker != null)
                seeker.enabled = false;

            var characterController = GetComponent<CharacterController> ();
            Destroy (characterController);

            FireOnStartClient ();
        }
        #endif
    }
}