using System;
using CosmicChamps.Battle.Units.Effects;
using UniRx;
using UnityEngine;

namespace CosmicChamps.Battle.Units.AnimatorCustomization
{
    public class RegularUnitProjectileAttackBehaviour : UnitAttackBehaviour
    {
        [SerializeField]
        private RegularUnitNetworkBehaviour _unit;

        [SerializeField]
        private ProjectileAttackEffect _effect;

        [SerializeField]
        private float _delay;

        private IDisposable _timerSubscription;

        public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _timerSubscription = Observable
                .Timer (TimeSpan.FromSeconds (_delay))
                .Subscribe (_ => _effect.Activate (_unit.TargetPosition))
                .AddTo (animator);
        }

        public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _timerSubscription?.Dispose ();
            _timerSubscription = null;
        }

        private void OnDestroy ()
        {
            _timerSubscription?.Dispose ();
        }
    }
}