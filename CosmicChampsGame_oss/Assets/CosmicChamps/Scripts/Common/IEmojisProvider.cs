using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CosmicChamps.Common
{
    public interface IEmojisProvider
    {
        UniTask<Sprite> GetEmoji (string id);
    }
}