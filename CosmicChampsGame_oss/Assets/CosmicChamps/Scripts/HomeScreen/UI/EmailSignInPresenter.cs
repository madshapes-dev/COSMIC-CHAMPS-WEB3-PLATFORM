using System;
using CosmicChamps.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CosmicChamps.HomeScreen.UI
{
    public class EmailSignInPresenter : AbstractPresenter<Unit, EmailSignInPresenter.Callbacks, EmailSignInPresenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action<string, string, ProgressIcon> OnSignIn;
            public readonly Action OnSignUp;
            public readonly Action<string, ProgressIcon> OnResetPassword;

            public Callbacks (
                Action<string, string, ProgressIcon> onSignIn,
                Action onSignUp,
                Action<string, ProgressIcon> onResetPassword)
            {
                OnSignIn = onSignIn;
                OnSignUp = onSignUp;
                OnResetPassword = onResetPassword;
            }
        }


        [FormerlySerializedAs ("emailInput")]
        [SerializeField]
        private TMP_InputField _emailInput;

        [FormerlySerializedAs ("passwordInput")]
        [SerializeField]
        private TMP_InputField _passwordInput;

        [FormerlySerializedAs ("signInButton")]
        [SerializeField]
        private Button _signInButton;

        [FormerlySerializedAs ("signUpButton")]
        [SerializeField]
        private Button _signUpButton;

        [FormerlySerializedAs ("_progressIcon")]
        [SerializeField]
        private ProgressIcon _signInProgressIcon;

        [SerializeField]
        private Button _resetPasswordButton;

        protected override void Clear ()
        {
            _emailInput.text = string.Empty;
            _passwordInput.text = string.Empty;
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _signUpButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnSignUp ())
                .AddTo (_callbacksDisposables);

            _signInButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnSignIn (_emailInput.text, _passwordInput.text, _signInProgressIcon))
                .AddTo (_callbacksDisposables);

            _resetPasswordButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnResetPassword (_emailInput.text, _signInProgressIcon))
                .AddTo (_callbacksDisposables);
        }
    }
}