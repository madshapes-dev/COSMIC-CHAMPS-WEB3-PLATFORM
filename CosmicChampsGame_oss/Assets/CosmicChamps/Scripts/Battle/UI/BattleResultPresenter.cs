using System;
using System.Linq;
using CosmicChamps.Battle.Data;
using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.Battle.UI
{
    public class BattleResultPresenter : AbstractPresenter<BattleResultPresenter.Model, Unit, BattleResultPresenter>
    {
        public readonly struct Model
        {
            public readonly BattleFinishData BattleFinishData;

            public Model (BattleFinishData battleFinishData)
            {
                BattleFinishData = battleFinishData;
            }
        }

        [SerializeField]
        private TextMeshProUGUI _resultCaption;

        [SerializeField]
        private TextMeshProUGUI _messageCaption;

        [SerializeField]
        private Button _okButton;

        [SerializeField]
        private string _localizationTable;

        [SerializeField]
        private LocalizedString _battleResultVictory;

        [SerializeField]
        private LocalizedString _battleResultDefeat;

        [SerializeField]
        private LocalizedString _battleResultDrawn;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [Inject]
        private IMessageBroker _messageBroker;

        [Inject]
        private IBrandingFactory _brandingFactory;

        [Inject]
        private AdService _adService;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private UILocker _uiLocker;

        protected override void Awake ()
        {
            base.Awake ();

            _okButton
                .OnClickAsObservable ()
                .Subscribe (OnOkClicked)
                .AddTo (this);
        }

        private void OnOkClicked (Unit _)
        {
            async UniTaskVoid DisplayAd ()
            {
                if (_gameService.GetCachedPlayer ().GamesPlayed >= 3)
                    await _adService.Show ();

                _messageBroker.Publish (new RestartSignal ());
            }

            _okButton.SetVisible (false);
            DisplayAd ().Forget ();
        }

        public override async UniTask DisplayAsync (Model model, PresenterDisplayOptions options = PresenterDisplayOptions.Notify)
        {
            var resultText = await (model.BattleFinishData.Result switch
            {
                BattleResult.Victory => _battleResultVictory,
                BattleResult.Defeat => _battleResultDefeat,
                BattleResult.Drawn => _battleResultDrawn,
                _ => throw new ArgumentOutOfRangeException ()
            }).GetLocalizedStringAsync ();

            var messageText = await new LocalizedString (_battleResultVictory.TableReference, model.BattleFinishData.Message)
                .GetLocalizedStringAsync ();

            _resultCaption.text = resultText;
            _messageCaption.text = messageText;
            _messageCaption.SetVisible (!string.IsNullOrEmpty (messageText));

            var opponentBot = _gameService
                .GetCachedGameData ()
                .Bots
                .FirstOrDefault (x => x.PlayerId == model.BattleFinishData.OpponentId);

            if (opponentBot != null)
                await (model.BattleFinishData.Result == BattleResult.Victory
                    ? _brandingFactory.GetWin (opponentBot.Id, transform)
                    : _brandingFactory.GetLoss (opponentBot.Id, transform));

            await base.DisplayAsync (model, options);
        }
        #endif
    }
}