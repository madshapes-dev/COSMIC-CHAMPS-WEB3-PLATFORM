using CosmicChamps.Services;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace CosmicChamps.Battle.Units.AnimatorCustomization
{
    public class RamAttackBehaviour : UnitAttackBehaviour
    {
        [SerializeField]
        private RegularUnitNetworkBehaviour _unit;

        [Inject]
        private IGameService _gameService;

        public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var unitData = _gameService
                .GetCachedGameData ()
                .GetUnit (_unit.Id);

            base.OnStateEnter (animator, stateInfo, layerIndex);
            transform.parent.DOMove (_unit.TargetPosition, unitData.ViewParams.AttackDuration);
        }
    }
}