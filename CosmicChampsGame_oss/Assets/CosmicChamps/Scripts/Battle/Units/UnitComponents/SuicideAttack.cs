using System;
using System.Threading;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using CosmicChamps.Data;
using Cysharp.Threading.Tasks;
using Serilog;
using Zenject;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class SuicideAttack : IAttack
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private IUnit _unit;
        private ITargetSeeker _targetSeeker;
        private IAnimator _animator;
        private IDamager _damager;

        [Inject]
        private ILogger _logger;

        public async UniTask PerformAttack (CancellationToken softCancellationToken, CancellationToken totalCancellationToken)
        {
            var target = _targetSeeker.GetCurrentTarget ();
            var unitViewData = _unit.ViewParams;

            await _unit.AimTarget (
                target,
                unitViewData.TurnTargetDuration,
                softCancellationToken);

            _animator.Attack ();
            await UniTask.Delay (
                TimeSpan.FromSeconds (unitViewData.DamageDelay),
                cancellationToken: totalCancellationToken);

            _logger.Information ("Apply standard damage of {Damage}", _unit.Stats.Damage[_unit.Level]);
            _damager.ApplyDamage (_unit.Stats.Damage, _unit.Level);
            await UniTask.Delay (
                TimeSpan.FromSeconds (unitViewData.AttackDuration - unitViewData.DamageDelay),
                cancellationToken: totalCancellationToken);

            _unit.ApplyDamage (_unit.Hp.Value);
        }

        public IAttack Clone () => new SuicideAttack ();

        public void OnStartServer (IUnit unit)
        {
            _unit = unit;
            _targetSeeker = unit.GetUnitComponent<ITargetSeeker> ();
            _animator = unit.GetUnitComponent<IAnimator> ();
            _damager = unit.GetUnitComponent<IDamager> (IDamager.Default);
        }
        #endif
    }
}