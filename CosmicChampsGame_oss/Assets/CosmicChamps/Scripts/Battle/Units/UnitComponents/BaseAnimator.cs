using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using Mirror;
using UnityEngine;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class BaseAnimator : IAnimator
    {
        [SerializeField]
        private NetworkAnimator _networkAnimator;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private static readonly int AttackAnimatorParam = Animator.StringToHash ("Attack");

        public void Move ()
        {
        }

        public void Stand ()
        {
        }

        public void Attack ()
        {
            _networkAnimator.SetTrigger (AttackAnimatorParam);
        }

        public void Die ()
        {
        }

        public void Deploy ()
        {
        }

        public void SetMovementSpeed (float speed)
        {
        }

        public IAnimator Clone () => new BaseAnimator
        {
            _networkAnimator = _networkAnimator
        };
        #endif
    }
}