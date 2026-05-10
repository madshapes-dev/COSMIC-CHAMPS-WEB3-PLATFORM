using System.Linq;
using CosmicChamps.Battle.Units.Effects;
using CosmicChamps.Level;
using UnityEngine;
using Zenject;

namespace CosmicChamps.Battle.Units.AnimatorCustomization
{
    public class BaseAttackBehaviour : UnitAttackBehaviour
    {
        [SerializeField]
        private BaseUnitNetworkBehaviour _unit;

        [SerializeField]
        private TurretAttackEffect _effect;

        [Inject]
        private LevelData _levelData;

        public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var cannon = _levelData
                .GetBase (_unit.Team)
                .Cannons
                .FirstOrDefault (x => x.Type == _unit.Type);

            if (cannon == null)
                return;

            _effect.Activate (cannon.Muzzle, _unit.TargetPosition);
        }
    }
}