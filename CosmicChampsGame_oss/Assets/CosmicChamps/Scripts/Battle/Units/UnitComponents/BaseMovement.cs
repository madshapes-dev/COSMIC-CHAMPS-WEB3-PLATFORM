using System;
using System.Threading;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class BaseMovement : IMovement
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private IUnit _unit;
        private ITargetSeeker _targetSeeker;

        public async UniTask ApproachTarget (CancellationToken cancellationToken)
        {
            await _targetSeeker.SearchForNewTarget (cancellationToken);
        }

        public void Stop ()
        {
        }

        public void OnStartServer (IUnit unit)
        {
            _unit = unit;
            _targetSeeker = _unit.GetUnitComponent<ITargetSeeker> ();
        }

        public IMovement Clone () => new BaseMovement ();
        #endif
    }
}