using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using Cysharp.Threading.Tasks;
using Serilog;
using Zenject;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class SpellTargetSeeker : ITargetSeeker
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private readonly List<ITarget> _possibleTargets = new();

        private IUnit _unit;
        private ITarget _target;

        [Inject]
        private ITargetsProvider _targetsProvider;

        [Inject]
        private ILogger _logger;

        public ITargetSeeker Clone () => new SpellTargetSeeker
        {
            _unit = _unit,
            _target = _target
        };

        public UniTask<ITarget> SearchForNewTarget (CancellationToken cancellationToken)
        {
            _targetsProvider.GetTargetsFor (_unit.Team.GetOpposite (), _possibleTargets);
            _target = _possibleTargets.Count > 0 ? _possibleTargets[0] : null;

            _logger.Information (
                "SearchForNewTarget opposite team {Team} targets {Targets} target {Target}",
                _unit.Team.GetOpposite (),
                string.Join (", ", _possibleTargets.Select (x => x.UnitId)),
                _target);

            return UniTask.FromResult (_target);
        }

        public ITarget GetCurrentTarget () => _target;

        public bool IsCurrentTargetInRange () => true;

        public void OnStartServer (IUnit unit)
        {
            _unit = unit;
        }
        #endif
    }
}