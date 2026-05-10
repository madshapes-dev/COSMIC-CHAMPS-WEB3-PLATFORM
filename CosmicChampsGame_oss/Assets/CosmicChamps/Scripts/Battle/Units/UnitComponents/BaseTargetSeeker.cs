using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using CosmicChamps.Services;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using Zenject;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class BaseTargetSeeker : ITargetSeeker
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private readonly List<ITarget> _possibleTargets = new();

        [Inject]
        private ITargetsProvider _targetsProvider;

        [Inject]
        private IGameService _gameService;

        private IUnit _unit;
        private ITarget _target;

        private float GetTargetDistance (ITarget target)
        {
            var position = _unit.Position;
            var targetPosition = target.GetAttackPosition (_unit);
            var distance = (position - targetPosition.world.WithY (position.y)).magnitude;

            return distance;
        }

        public async UniTask<ITarget> SearchForNewTarget (CancellationToken cancellationToken)
        {
            var attackRange = _unit.Stats.Damage.Range;
            var gameData = _gameService.GetCachedGameData ();

            _target = null;
            _targetsProvider.GetTargetsFor (_unit, _possibleTargets);

            var bestDistance = float.MaxValue;
            foreach (var possibleTarget in _possibleTargets.Where (x => x is not BaseTarget))
            {
                if (possibleTarget.IsDying)
                    continue;

                var distance = GetTargetDistance (possibleTarget);
                if (distance > attackRange || distance > bestDistance)
                    continue;

                _target = possibleTarget;
                bestDistance = distance;
            }

            await UniTask.Delay (TimeSpan.FromSeconds (gameData.UnitRepathInterval), cancellationToken: cancellationToken);
            return _target;
        }

        public ITarget GetCurrentTarget () => _target;

        public bool IsCurrentTargetInRange () => GetTargetDistance (_target) <= _unit.Stats.Damage.Range;

        public ITargetSeeker Clone () => new BaseTargetSeeker ();

        public void OnStartServer (IUnit unit)
        {
            _unit = unit;
        }
        #endif
    }
}