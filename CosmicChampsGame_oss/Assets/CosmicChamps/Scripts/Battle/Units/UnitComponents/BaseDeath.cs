using System;
using System.Threading;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class BaseDeath : IDeath
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        public async UniTask Die (CancellationToken cancellationToken)
        {
            await UniTask.Delay (TimeSpan.FromSeconds (0.1f), cancellationToken: CancellationToken.None);
        }

        public IDeath Clone () => new BaseDeath ();
        #endif
    }
}