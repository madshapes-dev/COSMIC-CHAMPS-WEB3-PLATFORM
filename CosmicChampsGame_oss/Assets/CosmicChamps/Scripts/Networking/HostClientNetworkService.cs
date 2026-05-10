using System;
using CosmicChamps.Battle.PVE;
using CosmicChamps.Data;
using CosmicChamps.Networking.Messages;
using Mirror;
using ThirdParty.Extensions.Components;
using UniRx;
using UnityEngine;
using BattleData = CosmicChamps.Battle.Data;

namespace CosmicChamps.Networking
{
    #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
    public class HostClientNetworkService : IBotNetworkService, IDisposable
    {
        private readonly ObservableEvent<Unit> _battleStarting = new();
        private readonly ObservableEvent<BattleData.BattleInitData> _battleInitialized = new();
        private readonly ObservableEvent<BattleData.NextCardReplacement> _nextCardReplaced = new();
        private readonly ObservableEvent<BattleData.BattleStartedData> _battleStarted = new();
        private readonly ObservableEvent<Unit> _battleOvertime = new();
        private readonly ObservableEvent<BattleData.BattleJoinData> _battleJoined = new();
        private readonly ObservableEvent<BattleData.BattleSyncData> _battleSynced = new();
        private readonly ObservableEvent<bool> _forfeitPossible = new();
        private readonly ObservableEvent<EnergyGrowRate> _energyGrowRateAdjusted = new();
        private readonly ObservableEvent<BattleData.BattleFinishData> _battleFinished = new();

        public IObservable<Unit> BattleStarting => _battleStarting.AsObservable ();
        public IObservable<BattleData.BattleInitData> BattlePlayerInitialized => _battleInitialized.AsObservable ();
        public IObservable<BattleData.NextCardReplacement> NextCardReplaced => _nextCardReplaced.AsObservable ();
        public IObservable<BattleData.BattleStartedData> BattleStarted => _battleStarted.AsObservable ();
        public IObservable<Unit> BattleOvertime => _battleOvertime.AsObservable ();
        public IObservable<BattleData.BattleJoinData> BattleJoined => _battleJoined.AsObservable ();
        public IObservable<BattleData.BattleSyncData> BattleSynced => _battleSynced.AsObservable ();
        public IObservable<bool> ForfeitPossible => _forfeitPossible.AsObservable ();
        public IObservable<EnergyGrowRate> EnergyGrowRateAdjusted => _energyGrowRateAdjusted.AsObservable ();
        public IObservable<BattleData.BattleFinishData> BattleFinished => _battleFinished.AsObservable ();


        private void OnBattleStarting (BattleStartingMessage _)
        {
            _battleStarting.Fire (Unit.Default);
        }

        private void OnBattleInitialized (BattleInitializedMessage message)
        {
            _battleInitialized.Fire (message.BattleInitData);
        }

        private void OnBattleStarted (BattleStartedMessage message)
        {
            _battleStarted.Fire (message.BattleStartedData);
        }

        private void OnBattleOvertime (BattleOvertimeMessage message)
        {
            _battleOvertime.Fire (Unit.Default);
        }

        private void OnBattleJoined (BattleJoinedMessage message)
        {
            _battleJoined.Fire (message.BattleJoinData);
        }

        private void OnNextCardReplaced (NextCardReplacedMessage message)
        {
            _nextCardReplaced.Fire (message.NextCardReplacement);
        }

        private void OnBattleSynced (BattleSyncMessage message)
        {
            _battleSynced.Fire (message.BattleSyncData);
        }

        private void OnForfeitPossible (ForfeitPossibleMessage message)
        {
            _forfeitPossible.Fire (message.Possible);
        }

        private void OnEnergyGrowRateAdjusted (AdjustEnergyGrowRateMessage message)
        {
            _energyGrowRateAdjusted.Fire (message.EnergyGrowRate);
        }

        private void OnBattleFinished (BattleFinishedMessage message)
        {
            _battleFinished.Fire (message.BattleFinishData);
        }

        private void RegisterMessageHandlers ()
        {
            NetworkClient.RegisterHandler<BattleStartingMessage> (OnBattleStarting);
            NetworkClient.RegisterHandler<BattleInitializedMessage> (OnBattleInitialized);
            NetworkClient.RegisterHandler<NextCardReplacedMessage> (OnNextCardReplaced);
            NetworkClient.RegisterHandler<BattleStartedMessage> (OnBattleStarted);
            NetworkClient.RegisterHandler<BattleOvertimeMessage> (OnBattleOvertime);
            NetworkClient.RegisterHandler<BattleJoinedMessage> (OnBattleJoined);
            NetworkClient.RegisterHandler<BattleSyncMessage> (OnBattleSynced);
            NetworkClient.RegisterHandler<ForfeitPossibleMessage> (OnForfeitPossible);
            NetworkClient.RegisterHandler<AdjustEnergyGrowRateMessage> (OnEnergyGrowRateAdjusted);
            NetworkClient.RegisterHandler<BattleFinishedMessage> (OnBattleFinished);
        }

        public void Dispose ()
        {
            Stop ();
        }

        public void Start (string id)
        {
            NetworkClient.ConnectHost ();
            NetworkClient.connection.isAuthenticated =
                NetworkServer.localConnection.isAuthenticated = true;
            NetworkServer.localConnection.authenticationData = new AuthData
            {
                PlayerId = id,
                PlayerSessionId = $"PlayerSession-{id}"
            };
            RegisterMessageHandlers ();
            NetworkClient.ConnectLocalServer ();
            NetworkClient.Ready ();
        }

        public void Stop ()
        {
            NetworkClient.Disconnect ();
            NetworkClient.Shutdown ();
        }

        public void LevelLoaded () => NetworkClient.Send (new ClientLevelLoadedMessage ());

        public void SpawnUnit (BattleData.Card card, Vector3 position) =>
            NetworkClient.Send (
                new UnitSpawningMessage
                {
                    UnitSpawningData = new BattleData.UnitSpawningData
                    {
                        Card = card,
                        Position = position
                    }
                });
    }
    #endif
}