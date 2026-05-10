using CosmicChamps.Services;
using CosmicChamps.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class PlayerLevelUpPresenter : AbstractPresenter
    {
        [SerializeField]
        private TextMeshProUGUI _levelCaption;

        [SerializeField]
        private TextMeshProUGUI _hpCaption;

        [SerializeField]
        private TextMeshProUGUI _damageCaption;

        [SerializeField]
        private Button _closeButton;

        [Inject]
        private IGameService _gameService;

        protected override void Awake ()
        {
            base.Awake ();
            _closeButton.OnClickAsObservable ().Subscribe (_ => Hide ()).AddTo (this);
        }

        public override void ForceRefresh ()
        {
            var player = _gameService.GetCachedPlayer ();
            var gameData = _gameService.GetCachedGameData ();
            var unitStats = gameData.BaseUnits[gameData.ShipCards[player.ShipSlot.Id].MainId].Stats;
            _levelCaption.text = player.Level.Value.ToString ();
            _hpCaption.text = unitStats.Hp[player.Level.Value].ToString ();
            _damageCaption.text = unitStats.Damage[player.Level.Value].ToString ();
        }

        public override void ForceClear ()
        {
        }
    }
}