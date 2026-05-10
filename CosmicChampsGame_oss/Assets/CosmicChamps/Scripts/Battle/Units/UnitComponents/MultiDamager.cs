using System;
using System.Collections.Generic;
using System.Linq;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using CosmicChamps.Data;
using Serilog;
using ThirdParty.Extensions;
using Zenject;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class MultiDamager : IDamager
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private readonly List<ITarget> _possibleTargets = new();

        private IUnit _unit;

        [Inject]
        private ITargetsProvider _targetsProvider;

        [Inject]
        private ILogger _logger;

        public void ApplyDamage (Damage damage, int damagerLevel)
        {
            float GetDistanceToTarget (ITarget target)
            {
                var attackerPosition = _unit.Position;
                var targetPosition = target.GetAttackPosition (_unit).world.WithY (attackerPosition.y);
                return (targetPosition - attackerPosition).magnitude;
            }

            bool IsTargetInRange (ITarget target) => GetDistanceToTarget (target) <= damage.Range;

            _targetsProvider.GetTargetsFor (_unit, _possibleTargets);
            var damageableTargetsInRange =
                _possibleTargets.Where (x => IsTargetInRange (x) && damage.SplashVictims.HasFlag (x.Type));

            if (_unit.Id == "fireball" || _unit.Id == "testspell")
            {
                _logger.Information (
                    $"fireball possibleTargets {string.Join (", ", _possibleTargets.Select (x => $"{x.UnitId}-{GetDistanceToTarget (x)}"))}");
                
                _logger.Information (
                    $"fireball damageableTargetsInRange {string.Join (", ", damageableTargetsInRange.Select (x => $"{x.UnitId}-{GetDistanceToTarget (x)}"))}");
            }

            foreach (var target in damageableTargetsInRange)
            {
                target.ApplyDamage (damage[damagerLevel]);
            }
        }

        public IDamager Clone () => new MultiDamager ();

        public void OnStartServer (IUnit unit)
        {
            _unit = unit;
        }
        #endif
    }
}