using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CosmicChamps.Battle.Units.AnimatorCustomization
{
    public class DiggDeployBehaviour : UnitDeployBehaviour
    {
        [Serializable]
        private class Effect
        {
            [SerializeField]
            private GameObject _gameObject;

            [SerializeField]
            private float _delay;

            public void Start (CancellationToken cancellationToken)
            {
                async UniTaskVoid InternalStart ()
                {
                    await UniTask.Delay (TimeSpan.FromSeconds (_delay), cancellationToken: cancellationToken);
                    _gameObject.SetActive (true);
                }

                InternalStart ().Forget ();
            }

            public void Stop ()
            {
                _gameObject.SetActive (false);
            }
        }

        [SerializeField]
        private Effect[] _effects;

        private CancellationTokenSource _cancellationTokenSource;

        public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter (animator, stateInfo, layerIndex);

            _cancellationTokenSource = new CancellationTokenSource ();
            foreach (var effect in _effects)
            {
                effect.Start (_cancellationTokenSource.Token);
            }
        }

        public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit (animator, stateInfo, layerIndex);

            _cancellationTokenSource.Cancel (false);

            foreach (var effect in _effects)
            {
                effect.Stop ();
            }
        }
    }
}