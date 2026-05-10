using System;
using CosmicChamps.UI;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CosmicChamps.HomeScreen.UI
{
    public class LandingPresenter : AbstractPresenter<Unit, LandingPresenter.Callbacks, LandingPresenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action OnEmailSignUp;
            public readonly Action OnSignIn;
            public readonly Action<ProgressIcon> OnGuestSignIn;
            public readonly Action OnImmutableSignIn;

            public Callbacks (
                Action onEmailSignUp,
                Action onSignIn,
                Action<ProgressIcon> onGuestSignIn,
                Action onImmutableSignIn)
            {
                OnEmailSignUp = onEmailSignUp;
                OnSignIn = onSignIn;
                OnGuestSignIn = onGuestSignIn;
                OnImmutableSignIn = onImmutableSignIn;
            }
        }

        [FormerlySerializedAs ("_signUpButton")]
        [SerializeField]
        private Button _emailSignUpButton;

        [SerializeField]
        private Button _signInButton;

        [SerializeField]
        private Button _guestSignInButton;

        [SerializeField]
        private Button _immutableSignInButton;

        [SerializeField]
        private ProgressIcon _guestSignInProgressIcon;

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _emailSignUpButton.OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnEmailSignUp ())
                .AddTo (this);
            _signInButton.OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnSignIn ())
                .AddTo (this);
            _guestSignInButton.OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnGuestSignIn (_guestSignInProgressIcon))
                .AddTo (this);
            _immutableSignInButton.OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnImmutableSignIn ())
                .AddTo (this);
        }
    }
}