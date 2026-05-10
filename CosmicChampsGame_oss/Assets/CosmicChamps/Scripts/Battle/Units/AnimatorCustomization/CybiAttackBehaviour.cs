using System;
using CosmicChamps.Battle.Units.Effects;
using UniRx;
using UnityEngine;

namespace CosmicChamps.Battle.Units.AnimatorCustomization
{
    public class CybiAttackBehaviour : UnitAttackBehaviour
    {
        [SerializeField]
        private CybiAttackEffect _effect;

        [SerializeField]
        private float _delay;

        private IDisposable _timerSubscription;

        public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _timerSubscription = Observable
                .Timer (TimeSpan.FromSeconds (_delay))
                .Subscribe (_ => _effect.Activate ())
                .AddTo (animator);
        }

        public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _timerSubscription?.Dispose ();
            _timerSubscription = null;
        }
    }
}