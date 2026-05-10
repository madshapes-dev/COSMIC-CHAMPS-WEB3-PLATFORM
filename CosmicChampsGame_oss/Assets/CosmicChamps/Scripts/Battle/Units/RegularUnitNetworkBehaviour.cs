using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using CosmicChamps.Battle.Units.UnitComponents.Setups;
using CosmicChamps.Common;
using CosmicChamps.Data;
using CosmicChamps.Networking;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Battle.Units
{
    public class RegularUnitNetworkBehaviour : AbstractUnitNetworkBehaviour
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        public class ServerFactory
        {
            private readonly IUnitViewDataProvider _unitViewDataProvider;
            private readonly DiContainer _container;
            private readonly ILogger _logger;

            public ServerFactory (IUnitViewDataProvider unitViewDataProvider, DiContainer container, ILogger logger)
            {
                _unitViewDataProvider = unitViewDataProvider;
                _container = container;
                _logger = logger;
            }

            private void AddUnitComponent<T> (
                    RegularUnitNetworkBehaviour instance,
                    UnitComponentSetup<T> unitComponentSetup,
                    string id = IUnitComponent.NoId)
                //
                where T : IUnitComponent<T>
            {
                var prototype = unitComponentSetup.Prototype;
                if (prototype == null)
                    return;

                var component = prototype.Clone ();
                _container.Inject (component);
                instance.AddUnitComponent (component, id);
            }

            public async UniTask<RegularUnitNetworkBehaviour> Create (
                UnitData unitData,
                int level,
                UnitBoost unitBoost,
                Vector3 position,
                Quaternion rotation)
            {
                var unitPrefab = await _unitViewDataProvider.GetPrefab (unitData.Id);
                if (unitPrefab == null)
                    throw new Exception ($"Prefab {unitData.Id}  not found");

                var instance = _container.InstantiatePrefabForComponent<RegularUnitNetworkBehaviour> (
                    unitPrefab,
                    position,
                    rotation,
                    null);
                instance._unitId = unitData.Id;
                instance._unitData = unitData;
                instance._level = level;
                instance._stats = unitBoost == null ? unitData.Stats : new BoostedUnitStats (unitData.Stats, unitBoost);

                _logger.Information (
                    "Create unitBoost {@UnitBoost} instance._stats {InstanceStats}",
                    unitBoost,
                    instance._stats.GetType ().Name);

                AddUnitComponent (instance, instance._animatorSetup);
                AddUnitComponent (instance, instance._attackSetup);
                AddUnitComponent (instance, instance._damagerSetup, IDamager.Default);
                AddUnitComponent (instance, instance._deathSetup);
                AddUnitComponent (instance, instance._deathDamagerSetup, IDamager.Death);
                AddUnitComponent (instance, instance._movementSetup);
                AddUnitComponent (instance, instance._targetSetup);
                AddUnitComponent (instance, instance._targetSeekerSetup);

                return instance;
            }
        }
        #endif

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        public class ClientFactory
        {
            private readonly IUnitViewDataProvider _unitViewDataProvider;
            private readonly ClientNetworkService _networkService;
            private readonly DiContainer _container;

            public ClientFactory (
                IUnitViewDataProvider unitViewDataProvider,
                DiContainer container,
                ClientNetworkService networkService)
            {
                _unitViewDataProvider = unitViewDataProvider;
                _container = container;
                _networkService = networkService;
            }

            private void RegisterUnitPrefab (DiContainer container, GameObject prefab)
            {
                _networkService.RegisterUnitPrefabPrefab (
                    prefab.gameObject,
                    container.InstantiatePrefabForComponent<RegularUnitNetworkBehaviour>);
            }

            public async UniTask Initialize (string[] unitIds)
            {
                foreach (var unitId in unitIds)
                {
                    var prefab = await _unitViewDataProvider.GetPrefab (unitId);
                    RegisterUnitPrefab (_container, prefab);
                }
            }
        }

        public class PreviewFactory
        {
            private readonly ICardViewDataProvider _cardViewDataProvider;

            public PreviewFactory (ICardViewDataProvider cardViewDataProvider)
            {
                _cardViewDataProvider = cardViewDataProvider;
            }

            public async UniTask<GameObject> Create (string cardId, string skin)
            {
                var previewPrefab = await _cardViewDataProvider.GetPreview (cardId, skin);
                var preview = Instantiate (previewPrefab);
                return preview;
            }
        }
        #endif

        [Header ("Regular Unit Bindings"), SerializeField]
        protected Transform _hpBarPlaceholder;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private IUnitStats _stats;
        public override IUnitStats Stats => _stats;
        #endif

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        public Vector3 BarPosition => _hpBarPlaceholder.position;
        #endif
    }
}