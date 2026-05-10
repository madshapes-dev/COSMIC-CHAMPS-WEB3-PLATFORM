using System;
using CosmicChamps.Bootstrap.Client.UI;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace CosmicChamps.Bootstrap.Client
{
    public class ErrorHandlingService : IInitializable, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly ErrorPresenter _errorPresenter;
        private readonly MaintenancePopup _maintenancePopup;
        private readonly IMessageBroker _messageBroker;
        private readonly UILocker _uiLocker;

        public ErrorHandlingService (
            ErrorPresenter errorPresenter,
            MaintenancePopup maintenancePopup,
            IMessageBroker messageBroker,
            UILocker uiLocker)
        {
            _errorPresenter = errorPresenter;
            _maintenancePopup = maintenancePopup;
            _messageBroker = messageBroker;
            _uiLocker = uiLocker;
        }

        private void OnOkClicked (ErrorSignal errorSignal)
        {
            if (errorSignal.Restart)
                _messageBroker.Publish (new RestartSignal ());

            _errorPresenter.Hide ();
        }

        private void OnErrorSignal (ErrorSignal errorSignal)
        {
            _uiLocker.Unlock ();
            _errorPresenter.Display (errorSignal);
        }

        private void OnMaintenance (MaintenanceSignal maintenanceSignal)
        {
            _maintenancePopup.Display (maintenanceSignal);
        }

        private void OnLogMessageReceived (string condition, string stacktrace, LogType type)
        {
            if (type != LogType.Exception || condition.StartsWith (nameof (SilentException)))
                return;

            OnErrorSignal (new ErrorSignal (condition, stacktrace, true, true));
        }

        public void Initialize ()
        {
            _errorPresenter.SetCallbacks (new ErrorPresenter.Callbacks (OnOkClicked));
            _messageBroker
                .Receive<ErrorSignal> ()
                .Subscribe (OnErrorSignal)
                .AddTo (_disposables);

            _messageBroker
                .Receive<MaintenanceSignal> ()
                .Subscribe (OnMaintenance)
                .AddTo (_disposables);

            Application.logMessageReceived += OnLogMessageReceived;
        }

        public void Dispose ()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
            _disposables.Dispose ();
        }
    }
}