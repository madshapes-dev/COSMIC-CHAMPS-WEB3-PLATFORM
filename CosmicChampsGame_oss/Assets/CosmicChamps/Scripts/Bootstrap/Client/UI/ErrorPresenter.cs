using System;
using System.Globalization;
using CosmicChamps.Data;
using CosmicChamps.Networking;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using ThirdParty.Extensions.Components;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.Bootstrap.Client.UI
{
    public class ErrorPresenter : AbstractPresenter<ErrorSignal, ErrorPresenter.Callbacks, ErrorPresenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action<ErrorSignal> OnOkClicked;

            public Callbacks (Action<ErrorSignal> onOkClicked)
            {
                OnOkClicked = onOkClicked;
            }
        }

        [FormerlySerializedAs ("messageText")]
        [SerializeField]
        private TextMeshProUGUI _messageText;

        [FormerlySerializedAs ("defaultMessage")]
        [SerializeField]
        private string _defaultMessage;

        [FormerlySerializedAs ("okButton")]
        [SerializeField]
        private Button _okButton;

        [FormerlySerializedAs ("_copyDetailsButton")]
        [SerializeField]
        private Button _reportErrorButton;

        [SerializeField]
        private ProgressIcon _progressIcon;

        [SerializeField]
        private TextMeshProUGUI _reportCompletedMessage;

        [SerializeField]
        private float _fadeDuration = 0.2f;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [Inject]
        private ClientNetworkService _clientNetworkService;

        [Inject]
        private GameRepository _gameRepository;

        [Inject]
        private UILocker _uiLocker;

        private async UniTaskVoid ReportErrorAsync ()
        {
            _uiLocker.Lock (_progressIcon);

            var gameSession = _clientNetworkService.PlayerGameSession;
            var gameSessionReport = gameSession != null
                ? $"Game Session: {gameSession.Id}\nnPlayer Session Id: {gameSession.PlayerSessionId}"
                : string.Empty;

            var reportId = await _gameRepository.ReportError (
                BuildInfo.AppVersionString,
                $"{Application.platform}(Client)",
                DateTime.Now.ToString (CultureInfo.InvariantCulture),
                gameSessionReport,
                model.Message,
                model.Stacktrace);

            UniClipboard.SetText (reportId);
            await _reportCompletedMessage.FadeIn (_fadeDuration);

            _uiLocker.Unlock ();
        }

        private void OnReportErrorClicked (Unit _)
        {
            ReportErrorAsync ().Forget ();
        }

        protected override void Refresh ()
        {
            _progressIcon.FadeOutImmediate ();
            _reportCompletedMessage.FadeOut (0f);
            _reportErrorButton.SetVisible (model.Reportable);

            _messageText.text = string.IsNullOrEmpty (model.Message) || model.HideMessage ? _defaultMessage : model.Message;
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);
            _okButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnOkClicked (model))
                .AddTo (_callbacksDisposables);

            _reportErrorButton
                .OnClickAsObservable ()
                .Subscribe (OnReportErrorClicked)
                .AddTo (_callbacksDisposables);
        }
        #endif
    }
}