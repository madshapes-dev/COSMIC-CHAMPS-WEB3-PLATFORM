using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CosmicChamps.Common
{
    public interface IShardsViewProvider
    {
        UniTask<Sprite> GetShardsIcon (string shardsId);
        UniTask SetDefaultTMPSprites ();
    }
}