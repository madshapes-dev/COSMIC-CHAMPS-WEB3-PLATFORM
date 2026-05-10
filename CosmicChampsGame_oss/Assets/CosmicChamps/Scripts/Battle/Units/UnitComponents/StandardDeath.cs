using System;
using System.Threading;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using Cysharp.Threading.Tasks;
using Pathfinding;
using UnityEngine;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class StandardDeath : IDeath
    {
        [SerializeField]
        private AIPath _ai;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private IUnit _unit;
        private IAnimator _animator;
        private IDamager _damager;

        [Inject]
        private ILogger _logger;

        public async UniTask Die (CancellationToken cancellationToken)
        {
            var unitViewData = _unit.ViewParams;

            _ai.enabled = false;

            _animator.Die ();
            await UniTask.Delay (
                TimeSpan.FromSeconds (unitViewData.DeathDamageDelay),
                cancellationToken: cancellationToken);

            if (_unit.Stats.DeathDamage != null)
            {
                _logger.Information ("Apply death damage of {Damage}", _unit.Stats.DeathDamage[_unit.Level]);
                _damager.ApplyDamage (_unit.Stats.DeathDamage, _unit.Level);
            }

            await UniTask.Delay (
                TimeSpan.FromSeconds (unitViewData.DeathDuration - unitViewData.DeathDamageDelay),
                cancellationToken: cancellationToken);
        }

        public IDeath Clone () => new StandardDeath
        {
            _ai = _ai
        };

        public void OnStartServer (IUnit unit)
        {
            _unit = unit;
            _animator = unit.GetUnitComponent<IAnimator> ();
            _damager = unit.GetUnitComponent<IDamager> (IDamager.Death);
        }
        #endif
    }
}