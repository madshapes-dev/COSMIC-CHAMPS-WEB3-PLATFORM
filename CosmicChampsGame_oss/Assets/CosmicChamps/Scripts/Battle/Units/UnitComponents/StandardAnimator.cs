using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using DG.Tweening;
using Mirror;
using UnityEngine;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class StandardAnimator : IAnimator
    {
        [SerializeField]
        private NetworkAnimator _networkAnimator;

        [SerializeField]
        private Collider _collider;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private static readonly int IsMovingAnimatorParam = Animator.StringToHash ("IsMoving");
        private static readonly int DieAnimatorParam = Animator.StringToHash ("Die");
        private static readonly int AttackAnimatorParam = Animator.StringToHash ("Attack");
        private static readonly int MoveSpeedParam = Animator.StringToHash ("MoveSpeed");
        private static readonly int DeployParam = Animator.StringToHash ("Deploy");

        private IUnit _unit;

        public void Move () => _networkAnimator.animator.SetBool (IsMovingAnimatorParam, true);

        public void Stand () => _networkAnimator.animator.SetBool (IsMovingAnimatorParam, false);

        public void Attack () => _networkAnimator.SetTrigger (AttackAnimatorParam);

        public void Die ()
        {
            var ray = new Ray (_collider.bounds.min, Vector3.down);
            if (Physics.Raycast (ray, out var hitInfo, float.MaxValue, Layers.Masks.Ground))
                _collider
                    .transform
                    .DOMoveY (_collider.bounds.min.y - _collider.transform.position.y + hitInfo.point.y, 0.2f);

            _networkAnimator.SetTrigger (DieAnimatorParam);
        }

        public void Deploy () => _networkAnimator.SetTrigger (DeployParam);

        public void SetMovementSpeed (float speed) =>
            _networkAnimator.animator.SetFloat (MoveSpeedParam, speed);

        public IAnimator Clone () => new StandardAnimator
        {
            _networkAnimator = _networkAnimator,
            _collider = _collider
        };

        public void OnStartServer (IUnit unit)
        {
            _unit = unit;
        }
        #endif
    }
}