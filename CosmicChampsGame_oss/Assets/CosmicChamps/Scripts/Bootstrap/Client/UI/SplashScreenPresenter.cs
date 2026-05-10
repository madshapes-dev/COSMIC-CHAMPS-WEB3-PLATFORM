using System;
using DG.Tweening;
using ThirdParty.Extensions;
using ThirdParty.Extensions.CanvasGroupFader;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Bootstrap.Client.UI
{
    public class SplashScreenPresenter : MonoBehaviour
    {
        [FormerlySerializedAs ("canvas")]
        [SerializeField]
        private Canvas _canvas;

        [FormerlySerializedAs ("messageText")]
        [SerializeField]
        private TextMeshProUGUI _messageText;

        [FormerlySerializedAs ("progress")]
        [SerializeField]
        private Transform _progress;

        [FormerlySerializedAs ("progressBar")]
        [SerializeField]
        private Image _progressBar;

        [FormerlySerializedAs ("defaultMessage")]
        [SerializeField]
        private string _defaultMessage = "Loading";

        [Inject]
        private ILogger _logger;

        [Inject]
        private IMessageBroker _messageBroker;

        private readonly CompositeDisposable disposables = new();
        private IDisposable textAnimationSubscription;
        private Tween fadeTween;

        private void OnDestroy ()
        {
            disposables.Dispose ();
        }

        public void Display (IReadOnlyReactiveProperty<float> progressProp = null)
        {
            Display (_defaultMessage, progressProp);
        }

        public void Display (string message, IReadOnlyReactiveProperty<float> progressProp = null)
        {
            _logger.Information ("Display {Message}", message);

            fadeTween?.Kill ();
            fadeTween = _canvas.FadeIn ();
            disposables.Clear ();

            if (string.IsNullOrEmpty (message))
                _messageText.text = null;
            else
                _messageText
                    .AnimateTextWithEndingDots (message)
                    .AddTo (disposables);

            if (progressProp != null)
            {
                _progress.SetVisible (true);
                progressProp
                    .Subscribe (x => _progressBar.fillAmount = x)
                    .AddTo (disposables);
            } else
            {
                _progress.SetVisible (false);
            }
        }

        public void Hide ()
        {
            void OnHidden () => disposables.Clear ();

            fadeTween?.Kill ();
            fadeTween = _canvas.FadeOut ();
            fadeTween.onComplete += OnHidden;
        }
    }
}