using System;
using CosmicChamps.Auth.Services;
using CosmicChamps.Bootstrap.Client.UI;
using CosmicChamps.Data;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;
using WalletBridges = CosmicChamps.Data.WalletBridges;

namespace CosmicChamps.Bootstrap.Client
{
    public class UpdateHandlingService : IInitializable, IDisposable
    {
        private static bool _softUpdatePopupShown;

        private readonly CompositeDisposable _disposables = new();

        private readonly UpdateRequiredPopup _popup;
        private readonly IMessageBroker _messageBroker;
        private readonly UILocker _uiLocker;
        private readonly Auth.Services.Auth.AuthClient _authClient;

        public UpdateHandlingService (
            UpdateRequiredPopup popup,
            IMessageBroker messageBroker,
            UILocker uiLocker,
            Auth.Services.Auth.AuthClient authClient)
        {
            _popup = popup;
            _messageBroker = messageBroker;
            _uiLocker = uiLocker;
            _authClient = authClient;
        }

        private void OnOkClicked (UpdateRequiredSignal signal)
        {
            Application.OpenURL (signal.Link);
            if (signal.Soft)
                _popup.Hide ();
        }

        private void OnContinueClicked (UpdateRequiredSignal obj)
        {
            _popup.Hide ();
        }

        private void OnUpdateRequiredSignal (UpdateRequiredSignal signal)
        {
            if (!signal.Soft)
            {
                _uiLocker.Unlock ();
                _popup.Display (signal);
                return;
            }

            if (_softUpdatePopupShown)
                return;

            _uiLocker.Unlock ();
            _popup.Display (signal);
            _softUpdatePopupShown = true;
        }

        public async UniTask<string> PerformVersionCheck () => await _authClient
            .VersionCheckAsync (new VersionCheckRequest ())
            .ResponseAsync
            .ContinueWith (x => x.Result.WalletBridgeUrl)
            .AsUniTask ();

        public void Initialize ()
        {
            _popup.SetCallbacks (new UpdateRequiredPopup.Callbacks (OnOkClicked, OnContinueClicked));
            _messageBroker
                .Receive<UpdateRequiredSignal> ()
                .Subscribe (OnUpdateRequiredSignal)
                .AddTo (_disposables);
        }

        public void Dispose () => _disposables.Dispose ();
    }
}