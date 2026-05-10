using System;

namespace CosmicChamps.ServerMatchmaking
{
    public interface IPreBattleService : IDisposable
    {
        void Initialize ();
    }
}