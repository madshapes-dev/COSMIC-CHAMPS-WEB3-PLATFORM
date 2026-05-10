using UnityEngine;

namespace CosmicChamps.Battle.Units.AnimatorCustomization
{
    public abstract class UnitStateMachineBehaviour<T> : StateMachineBehaviour where T : UnitAnimationBehaviour
    {
        private T _unitAttackBehaviour;

        private T GetUnitAnimationBehaviour (Animator animator)
        {
            if (_unitAttackBehaviour == null)
                _unitAttackBehaviour = animator.GetComponent<T> ();

            return _unitAttackBehaviour;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var unitAttackBehaviour = GetUnitAnimationBehaviour (animator);
            if (unitAttackBehaviour != null)
                unitAttackBehaviour.OnStateEnter (animator, stateInfo, layerIndex);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var unitAttackBehaviour = GetUnitAnimationBehaviour (animator);
            if (unitAttackBehaviour != null)
                unitAttackBehaviour.OnStateUpdate (animator, stateInfo, layerIndex);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var unitAttackBehaviour = GetUnitAnimationBehaviour (animator);
            if (unitAttackBehaviour != null)
                unitAttackBehaviour.OnStateExit (animator, stateInfo, layerIndex);
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        public override void OnStateMove (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var unitAttackBehaviour = GetUnitAnimationBehaviour (animator);
            if (unitAttackBehaviour != null)
                unitAttackBehaviour.OnStateMove (animator, stateInfo, layerIndex);
        }

        // OnStateIK is called right after Animator.OnAnimatorIK()
        public override void OnStateIK (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var unitAttackBehaviour = GetUnitAnimationBehaviour (animator);
            if (unitAttackBehaviour != null)
                unitAttackBehaviour.OnStateIK (animator, stateInfo, layerIndex);
        }
    }
}