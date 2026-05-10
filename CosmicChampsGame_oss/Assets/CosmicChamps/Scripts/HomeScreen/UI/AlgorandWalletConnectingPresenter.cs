using System;
using CosmicChamps.Services;
using CosmicChamps.Settings;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class AlgorandWalletConnectingPresenter :
        AbstractPresenter<
            AlgorandWalletConnectingPresenter.Model,
            AlgorandWalletConnectingPresenter.Callbacks,
            AlgorandWalletConnectingPresenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action OnConnected;
            public readonly Action OnClosed;

            public Callbacks (Action onConnected, Action onClosed)
            {
                OnConnected = onConnected;
                OnClosed = onClosed;
            }
        }

        public readonly struct Model
        {
        }

        [SerializeField]
        private Button _connectButton;

        [SerializeField]
        private Button _refreshButton;

        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private ProgressIcon _refreshProgress;

        [Inject]
        private AppProfile _appProfile;

        [Inject]
        private UILocker _uiLocker;

        [Inject]
        private IGameService _gameService;

        private Callbacks _callbacks;

        protected override void Awake ()
        {
            base.Awake ();

            _connectButton.OnClickAsObservable ().Subscribe (OnConnectClicked).AddTo (this);
            _refreshButton.OnClickAsObservable ().Subscribe (OnRefreshClicked).AddTo (this);
        }

        private void OpenConnectLink () => Application.OpenURL (_appProfile.ConnectWalletUrl);

        private void OnConnectClicked (Unit _) => OpenConnectLink ();

        private void OnRefreshClicked (Unit _)
        {
            async UniTaskVoid LoadPlayerData ()
            {
                _uiLocker.Lock (_refreshProgress);

                var player = await _gameService.LoadPlayerData ();
                if (!string.IsNullOrEmpty (player.LinkedWalletId))
                    _callbacks.OnConnected?.Invoke ();

                _uiLocker.Unlock ();
            }

            LoadPlayerData ().Forget ();
        }


        protected override void Refresh ()
        {
            OpenConnectLink ();
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);
            _callbacks = callbacks;

            _closeButton
                .OnClickAsObservable ()
                .Subscribe (_ => _callbacks.OnClosed?.Invoke ())
                .AddTo (_callbacksDisposables);
        }
    }
}