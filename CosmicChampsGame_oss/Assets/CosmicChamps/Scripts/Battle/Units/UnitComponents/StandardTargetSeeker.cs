using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using CosmicChamps.Data;
using Cysharp.Threading.Tasks;
using Pathfinding;
using ThirdParty.Extensions;
using UnityEngine;
using Zenject;
using MultiTargetPath = CosmicChamps.Battle.Server.MultiTargetPath;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class StandardTargetSeeker : ITargetSeeker
    {
        [SerializeField]
        protected AIPath _ai;

        [SerializeField]
        protected Seeker _seeker;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private readonly List<ITarget> _possibleTargets = new();

        private IUnit _unit;
        private ITarget _target;

        [Inject]
        private ITargetsProvider _targetsProvider;

        protected virtual IEnumerable<ITarget> FilterTargets (ICollection<ITarget> targets)
        {
            var damageTriggers = _unit.Stats.Damage.Triggers;
            var possibleTargets = targets.Where (x => damageTriggers.HasFlag (x.Type)).ToArray ();
            var unitTargetsInRange = possibleTargets
                .Where (x => x.Type != UnitTargetType.Base)
                .Select (x => (target: x, distance: (x.GetAttackPosition (_unit).world - _unit.Position).WithY (0f).magnitude))
                .Where (x => x.distance <= _unit.Stats.DetectRange)
                .ToArray ();

            if (unitTargetsInRange.Any ())
                return unitTargetsInRange.Select (x => x.target);

            var baseTargets = possibleTargets.Where (x => x.Type == UnitTargetType.Base).ToArray ();
            if (baseTargets.Any ())
                return baseTargets;

            throw new InvalidOperationException (
                $"No targets found; unitId: {_unit.Id}; PossibleTargetTypes: {damageTriggers}; targets: {string.Join (", ", targets.Select (x => $"{x.UnitId}/{x.Type}"))}; possibleTargets: {string.Join (", ", possibleTargets.Select (x => $"{x.UnitId}/{x.Type}"))}; baseTargets: {string.Join (", ", baseTargets.Select (x => $"{x.UnitId}/{x.Type}"))}");
        }

        private float GetTargetDistance (ITarget target)
        {
            var position = _unit.Position;
            var targetPosition = target.GetAttackPosition (_unit);
            var distance = (position - targetPosition.world.WithY (position.y)).magnitude;

            return distance;
        }

        public async UniTask<ITarget> SearchForNewTarget (CancellationToken cancellationToken)
        {
            _target = null;
            _targetsProvider.GetTargetsFor (_unit, _possibleTargets);

            var filteredTargets = FilterTargets (_possibleTargets).ToArray ();
            var targetPositions = filteredTargets.Select (x => x.GetAttackPosition (_unit).world).ToArray ();
            var path = MultiTargetPath.Construct (_unit.Position, targetPositions, _unit.Stats.Damage.Range, null);

            _seeker.StartPath (path);
            while (true)
            {
                await UniTask.Yield (cancellationToken);
                if (path.IsDone ())
                    break;
            }

            if (path.CompleteState != PathCompleteState.Complete)
                return null;

            _target = filteredTargets[path.chosenTarget];
            return _target;
        }

        public ITarget GetCurrentTarget () => _target;

        public bool IsCurrentTargetInRange () => GetTargetDistance (_target) <= _unit.Stats.Damage.Range;

        public virtual ITargetSeeker Clone () => new StandardTargetSeeker
        {
            _seeker = _seeker,
            _ai = _ai
        };

        public void OnStartServer (IUnit unit)
        {
            _unit = unit;
        }
        #endif
    }
}