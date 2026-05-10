using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CosmicChamps.Common
{
    public interface ICardViewDataProvider
    {
        UniTask<Sprite> GetCardSprite (string cardId, string unitSkinId);
        UniTask<GameObject> GetPreview (string cardId, string unitSkinId);
        UniTask PrewarmPreviews ((string, string)[] ids);
    }
}