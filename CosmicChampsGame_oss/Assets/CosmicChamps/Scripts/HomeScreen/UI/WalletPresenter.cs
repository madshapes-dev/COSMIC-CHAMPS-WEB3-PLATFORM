using System;
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
    public class WalletPresenter : AbstractPresenter<WalletPresenter.Model, WalletPresenter.Callbacks, WalletPresenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action OnConnectClicked;
            public readonly Action<ProgressIcon> OnDisconnectClicked;

            public Callbacks (Action onConnectClicked, Action<ProgressIcon> onDisconnectClicked)
            {
                OnConnectClicked = onConnectClicked;
                OnDisconnectClicked = onDisconnectClicked;
            }
        }

        public readonly struct Model
        {
            public readonly string WalletId;
            public readonly bool CanDisconnect;

            public Model (string walletId, bool canDisconnect)
            {
                WalletId = walletId;
                CanDisconnect = canDisconnect;
            }
        }

        [SerializeField]
        private RectTransform _connectedState;

        [SerializeField]
        private RectTransform _disconnectedState;

        [SerializeField]
        private Button _connectButton;

        [SerializeField]
        private Button _disconnectButton;

        [SerializeField]
        private Button _copyWalletIdButton;

        [SerializeField]
        private TextMeshProUGUI _walletIdCaption;

        [SerializeField]
        private ProgressIcon _disconnectProgressIcon;

        [Inject]
        private Captions _captions;

        protected override void Awake ()
        {
            base.Awake ();

            _copyWalletIdButton
                .OnClickAsObservable ()
                .Subscribe (OnCopyWalletIdClicked)
                .AddTo (this);
        }

        private string FormatWalletId () => model.WalletId.FormatWalletId ();

        private void OnCopyWalletIdClicked (Unit _)
        {
            async UniTaskVoid CopyPlayerId ()
            {
                _copyWalletIdButton.interactable = false;

                await _walletIdCaption.AnimateTextChangeThroughFade (_captions.WalletIdCopied);
                UniClipboard.SetText (model.WalletId);
                await UniTask.Delay (TimeSpan.FromSeconds (2f));
                await _walletIdCaption.AnimateTextChangeThroughFade (FormatWalletId ());

                _copyWalletIdButton.interactable = true;
            }

            CopyPlayerId ().Forget ();
        }

        protected override void Refresh ()
        {
            var isConnected = !string.IsNullOrEmpty (model.WalletId);

            _walletIdCaption.text = FormatWalletId ();

            _connectedState.SetVisible (isConnected);
            _disconnectedState.SetVisible (!isConnected);

            _disconnectButton.SetVisible (model.CanDisconnect);
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _connectButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnConnectClicked ())
                .AddTo (_callbacksDisposables);

            _disconnectButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnDisconnectClicked (_disconnectProgressIcon))
                .AddTo (_callbacksDisposables);
        }
    }
}