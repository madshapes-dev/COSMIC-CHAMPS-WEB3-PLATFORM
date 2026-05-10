using System;
using System.Collections.Generic;
using CosmicChamps.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;


namespace CosmicChamps.HomeScreen.UI
{
    public class ResetPasswordStep2Presenter :
        AbstractPresenter<ResetPasswordStep2Presenter.Model, ResetPasswordStep2Presenter.Callbacks, ResetPasswordStep2Presenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action<string, string, string, string, ProgressIcon> OnReset;
            public readonly Action OnBack;

            public Callbacks (Action<string, string, string, string, ProgressIcon> onReset, Action onBack)
            {
                OnReset = onReset;
                OnBack = onBack;
            }
        }

        public readonly struct Model
        {
            public readonly string Email;
            public readonly string CodeDestination;

            public Model (string email, string codeDestination)
            {
                Email = email;
                CodeDestination = codeDestination;
            }
        }

        [SerializeField]
        private TextMeshProUGUI _descriptionCaption;

        [SerializeField]
        private LocalizedString _descriptionLocalizedString;

        [SerializeField]
        private TMP_InputField _codeInput;

        [SerializeField]
        private TMP_InputField _passwordInput;

        [SerializeField]
        private TMP_InputField _confirmPasswordInput;

        [SerializeField]
        private Button _resetButton;

        [SerializeField]
        private Button _backButton;

        [SerializeField]
        private ProgressIcon _progressIcon;

        protected override void Clear ()
        {
            _codeInput.text = string.Empty;
            _passwordInput.text = string.Empty;
            _confirmPasswordInput.text = string.Empty;
        }

        protected override void Refresh ()
        {
            _descriptionLocalizedString.Arguments = new object[]
                { new Dictionary<string, string> { { "email", model.CodeDestination } } };

            _descriptionCaption.text = _descriptionLocalizedString.GetLocalizedString ();
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _resetButton
                .OnClickAsObservable ()
                .Subscribe (
                    _ => callbacks.OnReset (
                        model.Email,
                        _codeInput.text,
                        _passwordInput.text,
                        _confirmPasswordInput.text,
                        _progressIcon))
                .AddTo (_callbacksDisposables);

            _backButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnBack ())
                .AddTo (_callbacksDisposables);
        }
    }
}