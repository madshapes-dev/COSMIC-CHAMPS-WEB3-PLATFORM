using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.Settings;
using CosmicChamps.UI;
using CosmicChamps.Utils;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.HomeScreen.UI
{
    public class WalletButton : AbstractPresenter
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private float _fadeDuration = 0.2f;

        [SerializeField]
        private TextMeshProUGUI _caption;

        [Inject]
        private ILogger _logger;

        [Inject]
        private Captions _captions;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private WalletsPresenter _walletsPresenter;

        [Inject]
        private AppProfile _appProfile;

        [Inject]
        private GuestAccountConnectWalletPopup _guestAccountConnectWalletPopup;

        private readonly CompositeDisposable _disposables = new();

        protected override void Awake ()
        {
            base.Awake ();

            _button
                .OnClickAsObservable ()
                .Subscribe (OnButtonClicked)
                .AddTo (this);
        }

        private void OnButtonClicked (Unit unit)
        {
            if (_gameService.GetCachedPlayer ().AccountType == AccountType.Guest)
            {
                _guestAccountConnectWalletPopup.Display ();
                return;
            }

            _walletsPresenter.Display ();
            if (string.IsNullOrEmpty (_gameService.GetCachedPlayer ().LinkedWalletId))
                Application.OpenURL (_appProfile.ConnectWalletUrl);
        }

        private void SwitchToConnected (bool immediate)
        {
            _logger.Information ("SwitchToConnected");

            _caption.AnimateTextChangeThroughFade (
                _gameService.GetCachedPlayer ().LinkedWalletId.FormatWalletId (),
                immediate ? 0f : _fadeDuration);
        }

        private void SwitchToDisconnected (bool immediate)
        {
            _logger.Information ("SwitchToDisconnected");

            _caption.AnimateTextChangeThroughFade (
                _captions.ConnectWallet,
                immediate ? 0f : _fadeDuration);
        }

        private void OnPlayer (Player player, bool immediate)
        {
            var walletId = player?.LinkedWalletId;
            if (string.IsNullOrEmpty (walletId))
                SwitchToDisconnected (immediate);
            else
                SwitchToConnected (immediate);
        }

        public override void ForceRefresh ()
        {
            var player = _gameService.ObserveEveryValueChanged (x => x.GetCachedPlayer ()).ToReactiveProperty ();
            player
                .Skip (1)
                .Subscribe (x => OnPlayer (x, false))
                .AddTo (_disposables);

            OnPlayer (player.Value, true);
        }

        public override void ForceClear ()
        {
            _disposables.Clear ();
        }
    }
}