using System;
using ThirdParty.Extensions;
using UniRx;
using UnityEngine;

namespace CosmicChamps.Battle.Units.AnimatorCustomization
{
    public class PixieAttackBehaviour : UnitAttackBehaviour
    {
        [SerializeField]
        private ParticleSystem _effect;

        [SerializeField]
        private float _delay;

        private IDisposable _timerSubscription;

        public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _timerSubscription = Observable
                .Timer (TimeSpan.FromSeconds (_delay))
                .Subscribe (
                    _ =>
                    {
                        _effect.SetVisible (true);
                        _effect.Play ();
                    })
                .AddTo (animator);
        }

        public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _timerSubscription?.Dispose ();
            _timerSubscription = null;

            _effect.Stop ();
            _effect.Clear ();
            _effect.SetVisible (false);
        }
    }
}