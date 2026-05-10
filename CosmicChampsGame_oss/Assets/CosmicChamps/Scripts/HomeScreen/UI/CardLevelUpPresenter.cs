using CosmicChamps.Common;
using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using CosmicChamps.Utils;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class CardLevelUpPresenter : AbstractPresenter<
        CardLevelUpPresenter.Model,
        CardLevelUpPresenter.Callbacks,
        CardLevelUpPresenter>
    {
        public readonly struct Model
        {
            public readonly PlayerCard PlayerCard;
            public readonly string Skin;
            public readonly AbstractPresenter BackPresenter;

            public Model (PlayerCard playerCard, string skin, AbstractPresenter backPresenter)
            {
                PlayerCard = playerCard;
                Skin = skin;
                BackPresenter = backPresenter;
            }
        }

        public readonly struct Callbacks
        {
        }

        [SerializeField]
        private CardLevelUpResultPresenter _levelUpResultPresenter;

        [SerializeField]
        private TextMeshProUGUI _unitNameCaption;

        [SerializeField]
        private Image _avatar;

        [SerializeField]
        private TextMeshProUGUI _levelCaption;

        [SerializeField]
        private TextMeshProUGUI _nextLevelCaption;

        [SerializeField]
        private TextMeshProUGUI _hpCaption;

        [SerializeField]
        private TextMeshProUGUI _newHpCaption;

        [SerializeField]
        private TextMeshProUGUI _damageCaption;

        [SerializeField]
        private TextMeshProUGUI _newDamageCaption;

        [SerializeField]
        private TextMeshProUGUI _universalShardsCostCaption;

        [SerializeField]
        private TextMeshProUGUI _combinedCardShardsCostCaption;

        [SerializeField]
        private Image _combinedCardShardsCostIcon;

        [SerializeField]
        private TextMeshProUGUI _simpleCardShardsCostCaption;

        [SerializeField]
        private Image _simpleCardShardsCostIcon;

        [SerializeField]
        private Color _enoughColor;

        [SerializeField]
        private Color _notEnoughColor;

        [SerializeField]
        private Button _combinedLevelUpButton;

        [SerializeField]
        private ProgressIcon _combinedLevelUpProgressIcon;

        [SerializeField]
        private Button _simpleLevelUpButton;

        [SerializeField]
        private ProgressIcon _simpleLevelUpProgressIcon;

        [SerializeField]
        private Button _backButton;

        [Inject]
        private UILocker _uiLocker;

        [Inject]
        private ICardViewDataProvider _cardViewDataProvider;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private IMessageBroker _messageBroker;

        [Inject]
        private Captions _captions;

        [Inject]
        private IShardsViewProvider _shardsViewProvider;

        protected override void Awake ()
        {
            base.Awake ();

            _backButton
                .OnClickAsObservable ()
                .Subscribe (OnBackClicked)
                .AddTo (this);

            _combinedLevelUpButton
                .OnClickAsObservable ()
                .Subscribe (OnCombinedLevelUpClicked)
                .AddTo (this);

            _simpleLevelUpButton
                .OnClickAsObservable ()
                .Subscribe (OnSimpleLevelUpClicked)
                .AddTo (this);
        }

        private void Back ()
        {
            Hide ();
            model.BackPresenter.Display ();
        }

        private void OnCombinedLevelUpClicked (Unit _)
        {
            var player = _gameService.GetCachedPlayer ();
            var gameData = _gameService.GetCachedGameData ();
            var playerCard = model.PlayerCard;

            if (playerCard.LevelUpCapReached (player, gameData))
            {
                _messageBroker.Publish (
                    new ErrorSignal (_captions.Errors.CardLevelCapReached, string.Empty, false, false, false));
                return;
            }

            if (!playerCard.CombinedLevelUpPossible (player, gameData))
            {
                _messageBroker.Publish (new ErrorSignal (_captions.Errors.NotEnoughShards, string.Empty, false, false, false));
                return;
            }

            PerformLevelUp (_combinedLevelUpProgressIcon);
        }

        private void OnSimpleLevelUpClicked (Unit _)
        {
            var player = _gameService.GetCachedPlayer ();
            var gameData = _gameService.GetCachedGameData ();
            var playerCard = model.PlayerCard;

            if (playerCard.LevelUpCapReached (player, gameData))
            {
                _messageBroker.Publish (
                    new ErrorSignal (_captions.Errors.CardLevelCapReached, string.Empty, false, false, false));
                return;
            }

            if (!playerCard.SimpleLevelUpPossible (player, gameData))
            {
                _messageBroker.Publish (new ErrorSignal (_captions.Errors.NotEnoughShards, string.Empty, false, false, false));
                return;
            }

            PerformLevelUp (_simpleLevelUpProgressIcon);
        }

        private void PerformLevelUp (ProgressIcon progressIcon)
        {
            async UniTaskVoid LevelUp ()
            {
                _uiLocker.Lock (progressIcon);

                var player = _gameService.GetCachedPlayer ();
                var playerCard = model.PlayerCard;
                var cardLevelUpResult = await _gameService.CardLevelUp (playerCard.Id);

                player.UniversalShards = cardLevelUpResult.UniversalShards;
                player.CardShards[cardLevelUpResult.CardShards.Id] = cardLevelUpResult.CardShards;
                playerCard.Level.Value = cardLevelUpResult.Level;
                player.Level.Value = cardLevelUpResult.PlayerLevel;
                player.Exp.Value = cardLevelUpResult.PlayerExp;

                _uiLocker.Unlock ();

                await HideAsync ();
                await _levelUpResultPresenter.DisplayAsync (
                    new CardLevelUpResultPresenter.Model (model.PlayerCard, model.Skin, model.BackPresenter));
            }

            LevelUp ().Forget ();
        }

        private void OnBackClicked (Unit _) => Back ();

        private async UniTaskVoid LoadAvatar ()
        {
            var sprite = await _cardViewDataProvider.GetCardSprite (model.PlayerCard.Id, model.Skin);
            await _avatar.DOSpriteFade (sprite);
        }

        protected override void Refresh ()
        {
            base.Refresh ();

            var player = _gameService.GetCachedPlayer ();
            var gameData = _gameService.GetCachedGameData ();
            var playerCard = model.PlayerCard;
            var cardProgression = gameData.CardProgressions;
            var unitData = gameData.Units[model.Skin];
            var unitStats = unitData.Stats;
            var cardData = gameData.GetCard (model.PlayerCard.Id);
            var cardSkinData = cardData.GetSkin (model.Skin);
            var playerCardShards = player.GetCardShards (cardData.UpgradeShardId);

            _unitNameCaption.text = cardSkinData.DisplayName;

            _avatar.sprite = DOTweenModuleUI.TransparentPixelSprite;
            LoadAvatar ().Forget ();

            _levelCaption.text = _captions.CardLevel (model.PlayerCard.Level.Value);
            _nextLevelCaption.text = _captions.CardLevel (model.PlayerCard.Level.Value + 1);
            _hpCaption.text = unitStats.Hp[playerCard.Level.Value].ToString ();
            _newHpCaption.text = unitStats.Hp[playerCard.Level.Value + 1].ToString ();
            _damageCaption.text = unitStats.Damage[playerCard.Level.Value].ToString ();
            _newDamageCaption.text = unitStats.Damage[playerCard.Level.Value + 1].ToString ();

            var upgradeCost = cardProgression[playerCard.Level.Value].LevelUpCost;
            var universalShardsCost = upgradeCost - playerCardShards.Amount;

            _combinedCardShardsCostCaption.SetVisible (playerCardShards.Amount > 0);
            _combinedCardShardsCostCaption.text =
                $"<color=#{ColorUtility.ToHtmlStringRGB (_enoughColor)}>{playerCardShards.Amount.FormatShardsCost ()}</color>/{playerCardShards.Amount.FormatShardsCost ()}";

            _simpleCardShardsCostCaption.text =
                $"<color=#{ColorUtility.ToHtmlStringRGB (playerCardShards.Amount >= upgradeCost ? _enoughColor : _notEnoughColor)}>{playerCardShards.Amount.FormatShardsCost ()}</color>/{upgradeCost.FormatShardsCost ()}";

            _universalShardsCostCaption.text =
                $"<color=#{ColorUtility.ToHtmlStringRGB (player.UniversalShards >= universalShardsCost ? _enoughColor : _notEnoughColor)}>{player.UniversalShards.FormatShardsCost ()}</color>/{universalShardsCost.FormatShardsCost ()}";

            _combinedLevelUpButton.SetVisible (playerCardShards.Amount < upgradeCost);
        }

        public override async UniTask DisplayAsync (Model model, PresenterDisplayOptions options = PresenterDisplayOptions.Notify)
        {
            var cardData = _gameService.GetCachedGameData ().GetCard (model.PlayerCard.Id);

            _combinedCardShardsCostIcon.sprite =
                _simpleCardShardsCostIcon.sprite = await _shardsViewProvider.GetShardsIcon (cardData.UpgradeShardId);

            await base.DisplayAsync (model, options);
        }
    }
}