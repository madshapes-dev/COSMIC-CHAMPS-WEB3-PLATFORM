using System.Threading;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Battle.Units.UnitComponents.Abstract
{
    public interface IMovement : IUnitComponent<IMovement>
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        UniTask ApproachTarget (CancellationToken cancellationToken);
        void Stop ();
        #endif
    }
}