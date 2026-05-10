using CosmicChamps.Services;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Bootstrap.Client.UI
{
    public class MaintenancePopup : AbstractPresenter<MaintenanceSignal, Unit, MaintenancePopup>
    {
        [SerializeField]
        private Button _restartButton;

        [SerializeField]
        private Button _xButton;

        [SerializeField]
        private Button _telegramButton;

        [SerializeField]
        private Button _discordButton;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [Inject]
        private IMessageBroker _messageBroker;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private ILogger _logger;

        protected override void Awake ()
        {
            base.Awake ();

            _restartButton.OnClickAsObservable ()
                .Subscribe (
                    _ =>
                    {
                        Hide ();
                        _messageBroker.Publish (new RestartSignal ());
                    })
                .AddTo (this);
        }

        protected override void Refresh ()
        {
            _xButton
                .OnClickAsObservable ()
                .Subscribe (_ => Application.OpenURL (model.XUrl))
                .AddTo (_modelDisposables);
            _telegramButton
                .OnClickAsObservable ()
                .Subscribe (_ => Application.OpenURL (model.TelegramUrl))
                .AddTo (_modelDisposables);
            _discordButton
                .OnClickAsObservable ()
                .Subscribe (_ => Application.OpenURL (model.DiscordUrl))
                .AddTo (_modelDisposables);
        }
        #endif
    }
}