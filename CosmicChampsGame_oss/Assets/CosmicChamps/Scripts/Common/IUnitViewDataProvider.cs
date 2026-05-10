using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CosmicChamps.Common
{
    public interface IUnitViewDataProvider
    {
        UniTask<GameObject> GetPrefab (string id);
        UniTask Prewarm (string[] ids);
    }
}