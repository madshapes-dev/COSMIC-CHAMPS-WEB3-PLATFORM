using System;
using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using CosmicChamps.Utils;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ThirdParty.Extensions;
using ThirdParty.Extensions.CanvasGroupFader;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.HomeScreen.UI
{
    public class ProfilePresenter : AbstractPresenter
    {
        private readonly struct PlayerSnapshot
        {
            public readonly string Nickname;

            public PlayerSnapshot (Player player)
            {
                Nickname = player.Nickname.Value;
            }
        }

        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private TextMeshProUGUI _playerIdValue;

        [SerializeField]
        private float _playerIdCaptionFadeDuration = 0.2f;

        [SerializeField]
        private float _playerIdCopiedCaptionStayDuration = 2f;

        [SerializeField]
        private Button _copyPlayerIdButton;

        [SerializeField]
        private TextMeshProUGUI _nicknameCaption;

        [SerializeField]
        private Transform _emailValue;

        [SerializeField]
        private TextMeshProUGUI _emailCaption;

        [SerializeField]
        private Button _bindEmailButton;

        [SerializeField]
        private TMP_InputField _nicknameInput;

        [SerializeField]
        private Button _saveButton;

        [SerializeField]
        private ProgressIcon _saveProgress;

        [SerializeField]
        private RectTransform _profilePanel;

        [Header ("Sign Out Confirmation"), SerializeField]
        private Button _signOutButton;

        [SerializeField]
        private RectTransform _signOutConfirmationPanel;

        [SerializeField]
        private Button _signOutConfirmationButton;

        [SerializeField]
        private Button _signOutConfirmationBackButton;

        [Header ("Promo Code"), SerializeField]
        private Button _promoCodeButton;

        [SerializeField]
        private RectTransform _promoCodePanel;

        [SerializeField]
        private Button _promoCodeRedeemButton;

        [SerializeField]
        private Button _promoCodeCloseButton;

        [SerializeField]
        private TMP_InputField _promoCodeInput;

        [SerializeField]
        private ProgressIcon _redeemProgressIcon;

        [Header ("Promo Code Reward"), SerializeField]
        private RectTransform _promoCodeRewardPanel;

        [SerializeField]
        private TextMeshProUGUI _promoCodeRewardMessage;

        [SerializeField]
        private Button _promoCodeRewardCloseButton;

        [Inject]
        private IMessageBroker _messageBroker;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private UILocker _uiLocker;

        [Inject]
        private Captions _captions;

        [Inject]
        private BindEmailPresenter _bindEmailPresenter;

        [Inject]
        private ILogger _logger;

        private readonly CompositeDisposable _disposables = new();

        private Tween _playerIdCaptionTween;
        private PlayerSnapshot _playerSnapshot;

        protected override void Awake ()
        {
            base.Awake ();

            _signOutButton
                .OnClickAsObservable ()
                .Subscribe (OnSignOutButton)
                .AddTo (this);

            _copyPlayerIdButton
                .OnClickAsObservable ()
                .Subscribe (OnCopyPlayerIdClicked)
                .AddTo (this);

            _closeButton
                .OnClickAsObservable ()
                .Subscribe (_ => Hide ())
                .AddTo (this);

            _saveButton
                .OnClickAsObservable ()
                .Subscribe (OnSaveClicked)
                .AddTo (this);

            _bindEmailButton
                .OnClickAsObservable ()
                .Subscribe (OnBindEmailClicked)
                .AddTo (this);

            _bindEmailPresenter.SetCallbacks (
                new BindEmailPresenter.Callbacks (OnEmailBound));

            _signOutConfirmationButton
                .OnClickAsObservable ()
                .Subscribe (OnSignOutConfirmationClicked)
                .AddTo (this);

            _signOutConfirmationBackButton
                .OnClickAsObservable ()
                .Subscribe (OnSignOutConfirmationBackClicked)
                .AddTo (this);

            _promoCodeButton
                .OnClickAsObservable ()
                .Subscribe (OnPromoCodeClicked)
                .AddTo (this);

            _promoCodeCloseButton
                .OnClickAsObservable ()
                .Subscribe (OnPromoCodeCloseClicked)
                .AddTo (this);

            _promoCodeRedeemButton
                .OnClickAsObservable ()
                .Subscribe (OnPromoCodeRedeemClicked)
                .AddTo (this);

            _promoCodeRewardCloseButton
                .OnClickAsObservable ()
                .Subscribe (OnPromoCodeRewardCloseClicked)
                .AddTo (this);
        }

        private void OnPromoCodeRewardCloseClicked (Unit unit)
        {
            _promoCodeRewardPanel.FadeOut ();
            _profilePanel.FadeIn ();
        }

        private void OnPromoCodeRedeemClicked (Unit unit)
        {
            async UniTaskVoid Redeem ()
            {
                _uiLocker.Lock (_redeemProgressIcon);
                _promoCodeRewardMessage.text = await _gameService.RedeemPromoCode (_promoCodeInput.text);
                await UniTask.WhenAll (_promoCodePanel.FadeOut ().ToUniTask (), _promoCodeRewardPanel.FadeIn ().ToUniTask ());
                _uiLocker.Unlock ();
            }

            if (string.IsNullOrEmpty (_promoCodeInput.text))
            {
                _messageBroker.Publish (new ErrorSignal (_captions.Errors.EmptyPromoCode, string.Empty, false, false, false));
                return;
            }

            Redeem ().Forget ();
        }

        private void OnPromoCodeCloseClicked (Unit unit)
        {
            _promoCodePanel.FadeOut ();
            _profilePanel.FadeIn ();
        }

        private void OnPromoCodeClicked (Unit unit)
        {
            _promoCodeInput.text = null;
            _promoCodePanel.FadeIn ();
            _profilePanel.FadeOut ();
        }

        private void OnSignOutConfirmationClicked (Unit unit)
        {
            Hide ();
            _messageBroker.Publish (new SignedOutSignal ());
        }

        private void OnSignOutConfirmationBackClicked (Unit unit)
        {
            _signOutConfirmationPanel.FadeOut ();
            _profilePanel.FadeIn ();
        }

        private void OnEmailBound ()
        {
            _bindEmailPresenter.Hide ();
            Display ();
        }

        private void OnBindEmailClicked (Unit _)
        {
            Hide ();
            _bindEmailPresenter.Display (new BindEmailPresenter.Model (this));
        }

        private void OnSaveClicked (Unit _)
        {
            async UniTaskVoid SaveProfile ()
            {
                var newNickname = _nicknameInput.text;
                if (string.IsNullOrEmpty (newNickname))
                {
                    _messageBroker.Publish (ErrorSignal.CreateNonReportable (_captions.Errors.EmptyNickname));
                    return;
                }

                if (_playerSnapshot.Nickname != newNickname)
                {
                    _uiLocker.Lock (_saveProgress);

                    var nicknameChangeCount = await _gameService.SaveProfile (newNickname);
                    var player = _gameService.GetCachedPlayer ();

                    player.Nickname.Value = newNickname;
                    player.NicknameChangeCount.Value = nicknameChangeCount;

                    CreatePlayerSnapshot ();

                    _nicknameCaption.text = FormatNicknameCaption ();

                    _uiLocker.Unlock ();
                }
            }

            SaveProfile ().Forget ();
        }

        private string FormatPlayerId () =>
            _gameService
                .GetCachedPlayer ()
                .Id
                .FormatNickname (16);

        private string FormatNicknameCaption () =>
            _captions.ProfileNickname (("times", _gameService.GetCachedPlayer ().NicknameChangeCount.Value));

        private void OnCopyPlayerIdClicked (Unit _)
        {
            async UniTaskVoid CopyPlayerId ()
            {
                _copyPlayerIdButton.interactable = false;

                await _playerIdValue.AnimateTextChangeThroughFade (_captions.PlayerIdCopied, _playerIdCaptionFadeDuration);
                UniClipboard.SetText (_gameService.GetCachedPlayer ().Id);

                await UniTask.Delay (TimeSpan.FromSeconds (_playerIdCopiedCaptionStayDuration));
                await _playerIdValue.AnimateTextChangeThroughFade (
                    FormatPlayerId (),
                    _playerIdCaptionFadeDuration);

                await UniTask.Delay (TimeSpan.FromSeconds (_playerIdCaptionFadeDuration));

                _copyPlayerIdButton.interactable = true;
            }

            CopyPlayerId ().Forget ();
        }

        private void CreatePlayerSnapshot () => _playerSnapshot = new PlayerSnapshot (_gameService.GetCachedPlayer ());

        private void OnSignOutButton (Unit unit)
        {
            _profilePanel.FadeOut ();
            _signOutConfirmationPanel.FadeIn ();
        }

        public override void ForceRefresh ()
        {
            _profilePanel.FadeIn (true);
            _signOutConfirmationPanel.FadeOut (true);
            _promoCodePanel.FadeOut (true);
            _promoCodeRewardPanel.FadeOut (true);

            var player = _gameService.GetCachedPlayer ();

            CreatePlayerSnapshot ();

            player.NicknameChangeCount.Select (x => x > 0).Subscribe (x => _nicknameInput.interactable = x).AddTo (_disposables);

            _nicknameInput.text = player.Nickname.Value;
            _emailCaption.text = player.Email;
            _playerIdValue.text = FormatPlayerId ();
            _nicknameCaption.text = FormatNicknameCaption ();

            var isGuest = player.AccountType == AccountType.Guest;
            _emailValue.SetVisible (!isGuest);
            _bindEmailButton.SetVisible (isGuest);
        }

        public override void ForceClear ()
        {
            _logger.Debug ("ForceClear");

            _disposables.Clear ();
            _playerIdValue.DOKill ();
        }

        public override async UniTask DisplayAsync (PresenterDisplayOptions options = PresenterDisplayOptions.Default)
        {
            if (options.HasFlag (PresenterDisplayOptions.Notify))
                _onDisplaying.Fire (new PresenterVisibilityEventArgs (this, options));

            ForceRefresh ();
            await Activator.Activate (this, options.HasFlag (PresenterDisplayOptions.Immediate));
        }
    }
}