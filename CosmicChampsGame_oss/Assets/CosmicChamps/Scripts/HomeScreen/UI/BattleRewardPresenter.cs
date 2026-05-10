using System;
using System.Linq;
using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.UI;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class BattleRewardPresenter : AbstractPresenter<BattleRewardPresenter.Model, Unit, BattleRewardPresenter>
    {
        public readonly struct Model
        {
            public readonly PlayerBattleReward Reward;

            public Model (PlayerBattleReward reward)
            {
                Reward = reward;
            }
        }

        [SerializeField]
        private GridLayoutGroup _grid;

        [SerializeField]
        private TextMeshProUGUI _headerCaption;

        [SerializeField]
        private Transform _score;

        [SerializeField]
        private TextMeshProUGUI _oldScoreCaption;

        [SerializeField]
        private TextMeshProUGUI _newScoreCaption;

        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private LocalizedString _victoryString;

        [SerializeField]
        private LocalizedString _drawnString;

        [SerializeField]
        private LocalizedString _defeatString;

        [SerializeField]
        private LocalizedString _missionString;

        [SerializeField]
        private TMP_ColorGradient _victoryGradient;

        [SerializeField]
        private TMP_ColorGradient _drawnGradient;

        [SerializeField]
        private TMP_ColorGradient _defeatGradient;

        [SerializeField]
        private TMP_ColorGradient _missionGradient;

        [SerializeField]
        private TMP_ColorGradient _scoreIncGradient;

        [SerializeField]
        private TMP_ColorGradient _scoreDevGradient;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private InventoryIconPresenter.Factory _iconFactory;

        [Inject]
        private Captions _captions;

        protected override void Awake ()
        {
            base.Awake ();

            _closeButton
                .OnClickAsObservable ()
                .Subscribe (_ => Hide ())
                .AddTo (this);
        }

        protected override void Refresh ()
        {
            var gameData = _gameService.GetCachedGameData ();
            var reward = model.Reward;
            foreach (var shardId in gameData.InventoryShardsOrder)
            {
                var shard = reward.Shards.FirstOrDefault (x => x.Id == shardId);
                if (shard == null)
                    continue;

                var iconPresenter = _iconFactory.Create ().SetParent (_grid).AddTo (_modelDisposables);
                iconPresenter.model = new InventoryIconPresenter.Model (shard);
            }

            var (headerString, headerGradient) = reward.BattleResult switch
            {
                BattleResult.Victory => (_victoryString, _victoryGradient),
                BattleResult.Drawn => (_drawnString, _drawnGradient),
                BattleResult.Defeat => (_defeatString, _defeatGradient),
                BattleResult.Mission => (_missionString, _missionGradient),
                _ => throw new ArgumentOutOfRangeException ()
            };

            _headerCaption.text = headerString.GetLocalizedString ();
            _headerCaption.colorGradientPreset = headerGradient;

            _score.SetVisible (reward.NewRaring != reward.OldRaring);
            _oldScoreCaption.text = _captions.BattleRewardOldScore (reward.OldRaring);
            _newScoreCaption.text = _captions.BattleRewardNewScore (reward.NewRaring);
            _newScoreCaption.colorGradientPreset = reward.OldRaring < reward.NewRaring ? _scoreIncGradient : _scoreDevGradient;
        }
    }
}