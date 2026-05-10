using UnityEngine;

namespace CosmicChamps.Battle.Units.AnimatorCustomization
{
    public abstract class UnitAnimationBehaviour : MonoBehaviour
    {
        public virtual void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnStateMove (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnStateIK (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }
}