using CosmicChamps.Common;
using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class CardLevelUpResultPresenter : AbstractPresenter<
        CardLevelUpResultPresenter.Model,
        CardLevelUpResultPresenter.Callbacks,
        CardLevelUpResultPresenter>
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
        private TextMeshProUGUI _unitNameCaption;

        [SerializeField]
        private Image _avatar;

        [SerializeField]
        private TextMeshProUGUI _levelCaption;

        [SerializeField]
        private TextMeshProUGUI _hpCaption;

        [SerializeField]
        private TextMeshProUGUI _damageCaption;

        [SerializeField]
        private TextMeshProUGUI _expCaption;

        [SerializeField]
        private Button _backButton;

        [Inject]
        private ICardViewDataProvider _cardViewDataProvider;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private Captions _captions;

        protected override void Awake ()
        {
            base.Awake ();

            _backButton
                .OnClickAsObservable ()
                .Subscribe (OnBackClicked)
                .AddTo (this);
        }

        private void OnBackClicked (Unit _)
        {
            Hide ();
            model.BackPresenter.Display ();
        }

        private async UniTaskVoid LoadAvatar ()
        {
            var sprite = await _cardViewDataProvider.GetCardSprite (model.PlayerCard.Id, model.Skin);
            await _avatar.DOSpriteFade (sprite);
        }

        protected override void Refresh ()
        {
            base.Refresh ();

            var gameData = _gameService.GetCachedGameData ();
            var playerCard = model.PlayerCard;
            var unitData = gameData.Units[model.Skin];
            var unitStats = unitData.Stats;
            var cardData = gameData.GetCard (model.PlayerCard.Id);
            var cardSkinData = cardData.GetSkin (model.Skin);

            _unitNameCaption.text = cardSkinData.DisplayName;

            _avatar.sprite = DOTweenModuleUI.TransparentPixelSprite;
            LoadAvatar ().Forget ();

            _levelCaption.text = _captions.CardLevel (model.PlayerCard.Level.Value);
            _hpCaption.text = unitStats.Hp[playerCard.Level.Value].ToString ();
            _damageCaption.text = unitStats.Damage[playerCard.Level.Value].ToString ();
            _expCaption.text = $"+{gameData.CardProgressions[playerCard.Level.Value - 1].PlayerExp}";
        }
    }
}