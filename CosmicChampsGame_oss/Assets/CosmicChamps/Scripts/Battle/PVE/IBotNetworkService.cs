using System;
using UniRx;
using UnityEngine;
using BattleData = CosmicChamps.Battle.Data;

namespace CosmicChamps.Battle.PVE
{
    public interface IBotNetworkService
    {
        IObservable<Unit> BattleStarting { get; }
        IObservable<BattleData.BattleSyncData> BattleSynced { get; }
        IObservable<BattleData.NextCardReplacement> NextCardReplaced { get; }
        IObservable<BattleData.BattleInitData> BattlePlayerInitialized { get; }
        IObservable<BattleData.BattleFinishData> BattleFinished { get; }
        
        void LevelLoaded ();
        void SpawnUnit (BattleData.Card card, Vector3 position);
        
        void Stop ();
    }
}