using System;
using CosmicChamps.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicChamps.HomeScreen.UI
{
    public class SignInPresenter : AbstractPresenter<Unit, SignInPresenter.Callbacks, SignInPresenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action OnEmailSignIn;
            public readonly Action OnImmutableSignIn;
            public readonly Action OnSignUp;

            public Callbacks (Action onEmailSignIn, Action onImmutableSignIn, Action onSignUp)
            {
                OnEmailSignIn = onEmailSignIn;
                OnImmutableSignIn = onImmutableSignIn;
                OnSignUp = onSignUp;
            }
        }

        [SerializeField]
        private Button _emailSignIn;

        [SerializeField]
        private Button _immutableSignIn;

        [SerializeField]
        private Button _signUp;

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _emailSignIn
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnEmailSignIn ())
                .AddTo (_callbacksDisposables);

            _immutableSignIn
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnImmutableSignIn ())
                .AddTo (_callbacksDisposables);

            _signUp
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnSignUp ())
                .AddTo (_callbacksDisposables);
        }
    }
}