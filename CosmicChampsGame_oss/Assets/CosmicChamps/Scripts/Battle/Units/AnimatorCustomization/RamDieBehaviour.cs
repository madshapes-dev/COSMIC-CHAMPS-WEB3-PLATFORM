using UnityEngine;

namespace CosmicChamps.Battle.Units.AnimatorCustomization
{
    public class RamDieBehaviour : UnitDieBehaviour
    {
        [SerializeField]
        private GameObject _explosion;

        private void Awake ()
        {
            _explosion.SetActive (false);
        }

        public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter (animator, stateInfo, layerIndex);
            _explosion.SetActive (true);
        }
    }
}