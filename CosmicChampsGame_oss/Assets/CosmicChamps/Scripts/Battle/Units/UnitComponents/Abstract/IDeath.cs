using System.Threading;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Battle.Units.UnitComponents.Abstract
{
    public interface IDeath : IUnitComponent<IDeath>
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        UniTask Die (CancellationToken cancellationToken);
        #endif
    }
}