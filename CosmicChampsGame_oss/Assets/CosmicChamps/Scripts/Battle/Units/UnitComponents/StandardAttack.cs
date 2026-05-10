using System;
using System.Threading;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class StandardAttack : IAttack
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private IUnit _unit;
        private ITargetSeeker _targetSeeker;
        private IAnimator _animator;
        private IDamager _damager;

        public async UniTask PerformAttack (CancellationToken cancellationToken, CancellationToken totalCancellationToken)
        {
            bool IsCurrentTargetSuitable () => _targetSeeker.GetCurrentTarget () is { IsDying: false } &&
                                               _targetSeeker.IsCurrentTargetInRange ();

            var unitViewData = _unit.ViewParams;

            _animator.Stand ();

            while (true)
            {
                if (!IsCurrentTargetSuitable ())
                    return;

                await _unit.AimTarget (
                    _targetSeeker.GetCurrentTarget (),
                    unitViewData.TurnTargetDuration,
                    cancellationToken);

                _animator.Attack ();
                await UniTask.Delay (
                    TimeSpan.FromSeconds (unitViewData.DamageDelay),
                    cancellationToken: totalCancellationToken);

                _damager.ApplyDamage (_unit.Stats.Damage, _unit.Level);
                await UniTask.Delay (
                    TimeSpan.FromSeconds (unitViewData.AttackDuration - unitViewData.DamageDelay),
                    cancellationToken: totalCancellationToken);

                _animator.Stand ();

                if (!IsCurrentTargetSuitable ())
                    return;

                await UniTask.Delay (
                    TimeSpan.FromSeconds (_unit.Stats.Damage.Rate),
                    cancellationToken: cancellationToken);
            }
        }

        public IAttack Clone () => new StandardAttack ();

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