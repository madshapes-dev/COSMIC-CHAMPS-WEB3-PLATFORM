using System.Linq;
using CosmicChamps.Battle.Data;
using CosmicChamps.Battle.Units;
using CosmicChamps.Common;
using CosmicChamps.HomeScreen;
using CosmicChamps.Networking;
using CosmicChamps.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Battle.Client
{
    public class EntryPoint : MonoBehaviour
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [Inject]
        private BaseUnitNetworkBehaviour.ClientFactory _baseUnitFactory;

        [Inject]
        private ClientNetworkService _networkService;

        [Inject]
        private SoundsService _soundsService;

        [Inject]
        private RegularUnitNetworkBehaviour.ClientFactory _unitFactory;

        [Inject]
        private BattleStartingData _battleStartingData;

        [Inject]
        private ICardViewDataProvider _cardViewDataProvider;

        [Inject]
        private ILogger _logger;

        [Inject]
        private AdService _adService;

        private async void Start ()
        {
            _logger.Information (
                "_unitFactory.Initialize...{Cards}",
                string.Join (", ", _battleStartingData.Cards.Select (x => $"id: {x.Id}; skin: {x.Skin}")));
            await _unitFactory.Initialize (_battleStartingData.Cards.Select (x => x.Skin).ToArray ());
            await _cardViewDataProvider.PrewarmPreviews (_battleStartingData.Cards.Select (x => (x.Id, x.Skin)).ToArray ());

            _logger.Information ("_baseUnitFactory.Initialize...");
            _baseUnitFactory.Initialize ();
            _logger.Information ("_baseUnitFactory.LevelLoaded...");
            _networkService.LevelLoaded ();
            _soundsService.PlayBattleMusic ();

            _logger.Information ("EntryPoint.Start Done");

            _adService.Load ().Forget ();
        }
        #endif
    }
}