using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class InventoryPresenter : AbstractPresenter
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField]
        private GridLayoutGroup _grid;

        [SerializeField]
        private Button _closeButton;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private InventoryIconPresenter.Factory _iconFactory;

        protected override void Awake ()
        {
            base.Awake ();

            _closeButton
                .OnClickAsObservable ()
                .Subscribe (_ => Hide ())
                .AddTo (this);
        }

        public override UniTask DisplayAsync (PresenterDisplayOptions options = PresenterDisplayOptions.Notify)
        {
            var gameData = _gameService.GetCachedGameData ();
            var player = _gameService.GetCachedPlayer ();

            foreach (var shardsId in gameData.InventoryShardsOrder)
            {
                var cardShards = shardsId == gameData.UniversalShardsId
                    ? new PlayerCardShards
                    {
                        Id = gameData.UniversalShardsId,
                        Amount = player.UniversalShards
                    }
                    : player.GetCardShards (shardsId);

                var iconPresenter = _iconFactory.Create ().SetParent (_grid).AddTo (_disposables);
                iconPresenter.model = new InventoryIconPresenter.Model (cardShards);
            }

            return base.DisplayAsync (options);
        }

        public override void ForceRefresh ()
        {
        }

        public override void ForceClear ()
        {
            _disposables.Clear ();
        }
    }
}