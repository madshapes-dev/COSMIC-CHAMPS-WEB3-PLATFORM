using System;
using System.Threading;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class SpellMovement : IMovement
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private ITargetSeeker _targetSeeker;

        public IMovement Clone () => new SpellMovement
        {
            _targetSeeker = _targetSeeker
        };

        public UniTask ApproachTarget (CancellationToken cancellationToken) =>
            _targetSeeker.SearchForNewTarget (cancellationToken);

        public void Stop ()
        {
        }

        public void OnStartServer (IUnit unit)
        {
            _targetSeeker = unit.GetUnitComponent<ITargetSeeker> ();
        }
        #endif
    }
}