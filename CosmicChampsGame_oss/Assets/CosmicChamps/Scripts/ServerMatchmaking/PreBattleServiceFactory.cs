using System;
using CosmicChamps.Data;
using Zenject;

namespace CosmicChamps.ServerMatchmaking
{
    #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
    public class PreBattleServiceFactory
    {
        private readonly DiContainer _container;

        public PreBattleServiceFactory (DiContainer container)
        {
            _container = container;
        }

        public IPreBattleService Create (GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.PVP:
                    return _container.Instantiate<PVPPreBattleService> ();
                case GameMode.PVE:
                    return _container.Instantiate<PVEPreBattleService> ();
                default:
                    throw new ArgumentOutOfRangeException (nameof (gameMode), gameMode, null);
            }
        }
    }
    #endif
}