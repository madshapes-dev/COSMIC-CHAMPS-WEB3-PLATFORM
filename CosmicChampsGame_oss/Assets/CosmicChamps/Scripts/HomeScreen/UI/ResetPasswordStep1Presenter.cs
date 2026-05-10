using System;
using CosmicChamps.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicChamps.HomeScreen.UI
{
    public class ResetPasswordStep1Presenter :
        AbstractPresenter<Unit, ResetPasswordStep1Presenter.Callbacks, ResetPasswordStep1Presenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action<string, ProgressIcon> OnReset;
            public readonly Action OnBack;

            public Callbacks (Action<string, ProgressIcon> onReset, Action onBack)
            {
                OnReset = onReset;
                OnBack = onBack;
            }
        }

        [SerializeField]
        private TMP_InputField _emailInput;

        [SerializeField]
        private Button _resetButton;

        [SerializeField]
        private Button _backButton;

        [SerializeField]
        private ProgressIcon _progressIcon;

        protected override void Clear ()
        {
            _emailInput.text = string.Empty;
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _resetButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnReset (_emailInput.text, _progressIcon))
                .AddTo (_callbacksDisposables);

            _backButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnBack ())
                .AddTo (_callbacksDisposables);
        }
    }
}