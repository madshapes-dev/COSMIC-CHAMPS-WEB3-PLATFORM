using System;
using System.Threading;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class SpellDeath : IDeath
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        public IDeath Clone () => new SpellDeath ();

        public UniTask Die (CancellationToken cancellationToken) => UniTask.CompletedTask;
        #endif
    }
}