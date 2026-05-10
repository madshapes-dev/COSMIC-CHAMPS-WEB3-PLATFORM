using System.Threading;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Battle.Units.UnitComponents.Abstract
{
    public interface ITargetSeeker : IUnitComponent<ITargetSeeker>
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        UniTask<ITarget> SearchForNewTarget (CancellationToken cancellationToken);
        ITarget GetCurrentTarget ();
        bool IsCurrentTargetInRange ();
        #endif
    }
}