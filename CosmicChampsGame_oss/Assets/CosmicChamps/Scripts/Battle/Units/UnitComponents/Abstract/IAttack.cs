using System.Threading;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Battle.Units.UnitComponents.Abstract
{
    public interface IAttack : IUnitComponent<IAttack>
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        UniTask PerformAttack (CancellationToken softCancellationToken, CancellationToken totalCancellationToken);
        #endif
    }
}