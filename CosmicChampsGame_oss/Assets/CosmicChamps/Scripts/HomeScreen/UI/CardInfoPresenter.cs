using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CosmicChamps.Common;
using CosmicChamps.Data;
using CosmicChamps.HomeScreen.Model;
using CosmicChamps.Services;
using CosmicChamps.UI;
using CosmicChamps.Utils;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DOTweenModuleUI = ThirdParty.Extensions.DOTweenModuleUI;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.HomeScreen.UI
{
    public class CardInfoPresenter : AbstractPresenter<CardPresenterModel, Unit, CardInfoPresenter>
    {
        private class InternalModel
        {
            public readonly string Id;
            public string Skin;
            public readonly IList<string> Skins;

            public InternalModel (string id, string skin, IList<string> skins)
            {
                Id = id;
                Skin = skin;
                Skins = skins;
            }
        }

        [SerializeField]
        private CardLevelUpPresenter _cardLevelUpPresenter;

        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private TextMeshProUGUI _nameCaption;

        [SerializeField]
        private Image _avatar;

        [SerializeField]
        private TextMeshProUGUI _levelCaption;

        [SerializeField]
        private Progressbar _shardsProgressbar;

        [SerializeField]
        private TextMeshProUGUI _shardsProgressCaption;

        [SerializeField]
        private Transform _levelUpContainer;

        [SerializeField]
        private TextMeshProUGUI _levelUpButtonCaption;

        [SerializeField]
        private Button _levelUpButton;

        [SerializeField]
        private RectTransform _levelUpPossibleIcon;

        [SerializeField]
        private TextMeshProUGUI _rarityCaption;

        [SerializeField]
        private TextMeshProUGUI _rarityNumberCaption;

        [SerializeField]
        private TextMeshProUGUI _typeCaption;

        [SerializeField]
        private TextMeshProUGUI _healthCaption;

        [SerializeField]
        private TextMeshProUGUI _damageCaption;

        [SerializeField]
        private TextMeshProUGUI _hitChanceCaption;

        [SerializeField]
        private TextMeshProUGUI _movementSpeedCaption;

        [SerializeField]
        private TextMeshProUGUI _attackSpeedCaption;

        [SerializeField]
        private TextMeshProUGUI _criticalDamageCaption;

        [SerializeField]
        private TextMeshProUGUI _criticalHitChanceCaption;

        [SerializeField]
        private TextMeshProUGUI _dodgeCaption;

        [SerializeField]
        private TextMeshProUGUI _infoCaption;

        [Header ("Skin"), SerializeField]
        private Button _prevSkinButton;

        [SerializeField]
        private Button _nextSkinButton;

        [SerializeField]
        private Transform _skinsSwitcher;

        [Inject]
        private ICardViewDataProvider _cardViewDataProvider;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private ILogger _logger;

        [Inject]
        private Captions _captions;

        [Inject]
        private IMessageBroker _messageBroker;

        [Inject]
        private UILocker _uiLocker;

        private InternalModel _internalModel;

        protected override void Awake ()
        {
            base.Awake ();

            _closeButton
                .OnClickAsObservable ()
                .Subscribe (_ => Hide ())
                .AddTo (this);

            _prevSkinButton
                .OnClickAsObservable ()
                .Subscribe (OnPrevSkinButton)
                .AddTo (this);

            _nextSkinButton
                .OnClickAsObservable ()
                .Subscribe (OnNextSkinButton)
                .AddTo (this);

            _levelUpButton
                .OnClickAsObservable ()
                .Subscribe (OnLevelUpClicked)
                .AddTo (this);
        }

        private async UniTaskVoid LoadAvatar ()
        {
            var sprite = await _cardViewDataProvider.GetCardSprite (_internalModel.Id, _internalModel.Skin);
            await _avatar.DOSpriteFade (sprite);
        }

        private void OnSkinChanged ()
        {
            var cardData = _gameService.GetCachedGameData ().GetCard (_internalModel.Id);
            var cardSkinData = cardData.GetSkin (_internalModel.Skin);
            var unitData = _gameService.GetCachedGameData ().GetUnit (cardSkinData.Id);

            LoadAvatar ().Forget ();

            _nameCaption.AnimateTextChangeThroughFade (cardSkinData.DisplayName);
            _infoCaption.AnimateTextChangeThroughFade (cardSkinData.Description);
            _rarityCaption.AnimateTextChangeThroughFade (cardSkinData.Rarity);
            _rarityNumberCaption.AnimateTextChangeThroughFade (cardSkinData.RarityNumber);


            var player = _gameService.GetCachedPlayer ();
            var boost = player.GetBoost (unitData.BoostId);
            var playerCard = player.GetCard (_internalModel.Id);

            _healthCaption.AnimateTextChangeThroughFade (unitData.Stats.Hp[playerCard.Level.Value].FormatIntStat (boost?.Hp));
            _damageCaption.AnimateTextChangeThroughFade (
                unitData.Stats.Damage[playerCard.Level.Value].FormatIntStat (boost?.Damage));
            _hitChanceCaption.AnimateTextChangeThroughFade ("/");
            _movementSpeedCaption.AnimateTextChangeThroughFade (unitData.Stats.Speed.ToString (CultureInfo.InvariantCulture));
            _attackSpeedCaption.AnimateTextChangeThroughFade (
                (unitData.Stats.Damage.Rate + unitData.ViewParams.AttackDuration).ToString (CultureInfo.InvariantCulture));
            _criticalDamageCaption.AnimateTextChangeThroughFade ("/");
            _criticalHitChanceCaption.AnimateTextChangeThroughFade ("/");
            _dodgeCaption.AnimateTextChangeThroughFade ("/");
        }

        private void OnPrevSkinButton (Unit _)
        {
            var skinIndex = _internalModel.Skins.IndexOf (_internalModel.Skin) - 1;
            _internalModel.Skin = skinIndex < 0 ? _internalModel.Skins.Last () : _internalModel.Skins[skinIndex];

            if (model.IsDeckCard)
                model.AsDeckCard.Skin = _internalModel.Skin;

            OnSkinChanged ();
        }

        private void OnNextSkinButton (Unit _)
        {
            var skinIndex = (_internalModel.Skins.IndexOf (_internalModel.Skin) + 1) % _internalModel.Skins.Count;
            _internalModel.Skin = _internalModel.Skins[skinIndex];

            if (model.IsDeckCard)
                model.AsDeckCard.Skin = _internalModel.Skin;

            OnSkinChanged ();
        }

        private void RefreshLevelRelatedUI (bool immediate)
        {
            var animationDuration = immediate ? 0f : 0.2f;

            var player = _gameService.GetCachedPlayer ();
            var gameData = _gameService.GetCachedGameData ();

            var playerCard = player.GetCard (model.Id);
            var cardProgressions = _gameService.GetCachedGameData ().CardProgressions;
            var cardLevel = playerCard.Level.Value;
            var topLevel = cardLevel == cardProgressions.Length - 1;
            var cardData = gameData.GetCard (model.Id);
            var cardSkinData = cardData.GetSkin (model.Skin);
            var unitData = _gameService.GetCachedGameData ().GetUnit (cardSkinData.Id);
            var boost = player.GetBoost (unitData.BoostId);

            _levelCaption.AnimateTextChangeThroughFade (_captions.CardLevel (cardLevel), animationDuration);
            _healthCaption.text = unitData.Stats.Hp[cardLevel].FormatIntStat (boost?.Hp);
            _damageCaption.text = unitData.Stats.Damage[cardLevel].FormatIntStat (boost?.Damage);

            var cardShardsAmount = player.GetCardShards (cardData.UpgradeShardId).Amount;
            var levelUpCost = cardProgressions[cardLevel].LevelUpCost;

            _levelUpContainer.SetVisible (!topLevel);
            _shardsProgressbar.SetValue (topLevel ? 1f : Mathf.Clamp01 ((float)cardShardsAmount / levelUpCost), immediate);
            _shardsProgressCaption.AnimateTextChangeThroughFade (
                topLevel ? _captions.MaxLevel : $"{cardShardsAmount.FormatShardsCost ()}/{levelUpCost.FormatShardsCost ()}");
        }

        private void OnLevelUpClicked (Unit _)
        {
            var player = _gameService.GetCachedPlayer ();
            var playerCard = player.GetCard (model.Id);

            Hide ();
            _cardLevelUpPresenter.Display (new CardLevelUpPresenter.Model (playerCard, _internalModel.Skin, this));
        }

        protected override void Refresh ()
        {
            base.Refresh ();

            var gameData = _gameService.GetCachedGameData ();
            var player = _gameService.GetCachedPlayer ();
            var cardData = gameData.GetCard (model.Id);
            var playerCard = player.GetCard (model.Id);
            var cardSkinData = cardData.GetSkin (model.Skin);
            var unitData = _gameService.GetCachedGameData ().GetUnit (cardSkinData.Id);
            var playerUnit = player.GetUnit (cardData.UnitId);

            _internalModel = new InternalModel (model.Id, model.Skin, playerUnit.Skins);

            _avatar.sprite = DOTweenModuleUI.TransparentPixelSprite;
            LoadAvatar ().Forget ();

            RefreshLevelRelatedUI (true);

            _hitChanceCaption.text = "/"; //Random.Range (0, 100).ToString ();
            _movementSpeedCaption.text = unitData.Stats.Speed.ToString (CultureInfo.InvariantCulture);
            _attackSpeedCaption.text =
                (unitData.Stats.Damage.Rate + unitData.ViewParams.AttackDuration).ToString (CultureInfo.InvariantCulture);
            _criticalDamageCaption.text = "/"; //Random.Range (0, 100).ToString ();
            _criticalHitChanceCaption.text = "/"; //Random.Range (0, 100).ToString ();
            _dodgeCaption.text = "/"; //Random.Range (0, 100).ToString ();

            _nameCaption.text = cardSkinData.DisplayName;
            _infoCaption.text = cardSkinData.Description;

            _levelCaption.text = _captions.CardLevel (playerCard.Level.Value);
            _typeCaption.text = unitData.Type;
            _rarityCaption.text = cardSkinData.Rarity;
            _rarityNumberCaption.text = cardSkinData.RarityNumber;

            _levelUpPossibleIcon.SetVisible (playerCard.SimpleLevelUpPossible (player, gameData));

            var hasSkins = playerUnit.Skins.Length > 1;
            _skinsSwitcher.SetVisible (hasSkins);
        }
    }
}