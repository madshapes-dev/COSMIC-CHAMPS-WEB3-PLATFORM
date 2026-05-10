using System;
using System.Threading;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using CosmicChamps.Services;
using Cysharp.Threading.Tasks;
using Pathfinding;
using ThirdParty.Extensions;
using UnityEngine;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class StandardMovement : IMovement
    {
        [SerializeField]
        private Seeker _seeker;

        [SerializeField]
        private AIPath _ai;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        [Inject]
        private IGameService _gameService;

        [Inject]
        private ITimeProvider _timeProvider;

        [Inject]
        private ILogger _logger;

        private IUnit _unit;
        private ITargetSeeker _targetSeeker;
        private IAnimator _animator;

        public async UniTask ApproachTarget (CancellationToken cancellationToken)
        {
            _animator.Move ();

            var repathInterval = _gameService
                .GetCachedGameData ()
                .UnitRepathInterval;

            var lastRepathTime = 0f;
            while (true)
            {
                if (_unit.IsDying)
                    return;

                if (_timeProvider.Time - lastRepathTime >= repathInterval)
                {
                    lastRepathTime = _timeProvider.Time;
                    await _targetSeeker.SearchForNewTarget (cancellationToken);
                }

                var target = _targetSeeker.GetCurrentTarget ();
                if (target == null)
                {
                    await UniTask.Yield (cancellationToken);
                    continue;
                }

                var distance = (_unit.Position - target.GetAttackPosition (_unit).world)
                    .WithY (0f)
                    .magnitude;

                if (distance <= _unit.Stats.Damage.Range)
                {
                    _ai.SetPath (null);
                    // _logger.Information("{Unit} _ai.velocity.magnitude {Velocity}", _unit.Id, _ai.velocity.magnitude);
                    if (_ai.velocity.magnitude.CompareWithEps (0f) == 0)
                    {
                        /*DebugDrawer.DrawCircle (
                            _unit.Position.WithY (1f),
                            _unit.Stats.Damage.Range,
                            default,
                            Color.blue,
                            float.MaxValue);*/

                        return;
                    }
                }

                await UniTask.Yield (cancellationToken);
            }
        }

        public void Stop ()
        {
            _ai.canMove = false;
        }

        public IMovement Clone () => new StandardMovement
        {
            _seeker = _seeker,
            _ai = _ai
        };

        public void OnStartServer (IUnit unit)
        {
            _unit = unit;

            _targetSeeker = unit.GetUnitComponent<ITargetSeeker> ();
            _animator = unit.GetUnitComponent<IAnimator> ();

            _seeker.enabled = true;
            _seeker.graphMask = GraphMask.FromGraphName (_unit.MovementType.ToString ());

            _ai.endReachedDistance = 0f;
            _ai.maxSpeed = _unit.Stats.Speed;
        }
        #endif
    }
}