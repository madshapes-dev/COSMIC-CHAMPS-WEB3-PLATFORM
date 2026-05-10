using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.Settings;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions.CanvasGroupFader;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class WalletsPresenter : AbstractPresenter
    {
        [SerializeField]
        private RectTransform _optionsPanel;

        [SerializeField]
        private AlgorandWalletConnectingPresenter _algorandWalletConnectingPresenter;

        [SerializeField]
        private WalletPresenter _algorandWalletPresenter;

        [SerializeField]
        private WalletPresenter _immutableWalletPresenter;

        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private ProgressIcon _immutableProgress;

        [Inject]
        private AppProfile _appProfile;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private UILocker _uiLocker;

        [Inject]
        private Captions _captions;

        [Inject]
        private IImmutableService _immutableService;

        protected override void Awake ()
        {
            base.Awake ();

            _algorandWalletConnectingPresenter.SetCallbacks (
                new AlgorandWalletConnectingPresenter.Callbacks (
                    OnAlgorandConnectingConnected,
                    OnAlograndConnectingClosed));

            _algorandWalletPresenter.SetCallbacks (
                new WalletPresenter.Callbacks (OnAlgorandConnect, OnAlograndDisconnect));

            _immutableWalletPresenter.SetCallbacks (
                new WalletPresenter.Callbacks (OnImmutableConnect, OnImmutableDisconnect));

            _closeButton
                .OnClickAsObservable ()
                .Subscribe (OnCloseClicked)
                .AddTo (this);
        }

        private void OnImmutableConnect ()
        {
            async UniTaskVoid BindWallet ()
            {
                _uiLocker.Lock (_immutableProgress);

                await _immutableService.Login ();
                await _immutableService.RequestAccounts ();
                var immutableAccessToken = await _immutableService.GetAccessToken ();
                await _gameService.BindImmutableWallet (immutableAccessToken);
                RefreshImmutableWalletModel ();

                _uiLocker.Unlock ();
            }

            BindWallet ().Forget ();
        }

        private void OnImmutableDisconnect (ProgressIcon progressIcon)
        {
            async UniTaskVoid UnbindWallet ()
            {
                _uiLocker.Lock (progressIcon);
                await _gameService.UnbindImmutableWallet ();
                RefreshImmutableWalletModel ();
                _uiLocker.Unlock ();
            }

            UnbindWallet ().Forget ();
        }

        private void RefreshAlgorandWalletModel ()
        {
            _algorandWalletPresenter.model = new WalletPresenter.Model (
                _gameService.GetCachedPlayer ().LinkedWalletId,
                true);
        }

        private void RefreshImmutableWalletModel ()
        {
            var player = _gameService.GetCachedPlayer ();
            _immutableWalletPresenter.model = new WalletPresenter.Model (
                player.ImmutableWalletId,
                player.AccountType != AccountType.Immutable);
        }

        private void OnAlgorandConnect ()
        {
            _optionsPanel.FadeOut ();
            _algorandWalletConnectingPresenter.Display ();
        }

        private void OnAlograndDisconnect (ProgressIcon progressIcon)
        {
            async UniTaskVoid UnbindWallet ()
            {
                _uiLocker.Lock (progressIcon);
                await _gameService.UnbindAlgorandWallet ();
                RefreshAlgorandWalletModel ();
                _uiLocker.Unlock ();
            }

            UnbindWallet ().Forget ();
        }

        private void OnAlgorandConnectingConnected ()
        {
            RefreshAlgorandWalletModel ();
            _algorandWalletConnectingPresenter.Hide ();
            _optionsPanel.FadeIn ();
        }

        private void OnAlograndConnectingClosed ()
        {
            _algorandWalletConnectingPresenter.FadeOut ();
            _optionsPanel.FadeIn ();
        }

        private void OnCloseClicked (Unit _)
        {
            Hide ();
        }

        public override void ForceRefresh ()
        {
        }

        public override void ForceClear ()
        {
        }

        public override UniTask DisplayAsync (PresenterDisplayOptions options = PresenterDisplayOptions.Notify)
        {
            _algorandWalletConnectingPresenter.Hide (PresenterDisplayOptions.Immediate);
            _optionsPanel.FadeIn (true);

            RefreshAlgorandWalletModel ();
            RefreshImmutableWalletModel ();

            return base.DisplayAsync (options);
        }
    }
}