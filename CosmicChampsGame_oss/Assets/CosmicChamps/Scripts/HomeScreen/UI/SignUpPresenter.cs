using System;
using System.Linq;
using CosmicChamps.Services;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions.CanvasGroupFader;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class SignUpPresenter : AbstractPresenter<Unit, SignUpPresenter.Callbacks, SignUpPresenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action OnSignIn;

            public Callbacks (Action onSignIn)
            {
                OnSignIn = onSignIn;
            }
        }

        [Header ("Sign Up"), SerializeField]
        private Transform _signUpPanel;

        [SerializeField]
        private TMP_InputField _emailInput;

        [SerializeField]
        private TMP_InputField _passwordInput;

        [SerializeField]
        private TMP_InputField _confirmPasswordInput;

        [SerializeField]
        private Button _signUpButton;

        [SerializeField]
        private ProgressIcon _signUpProgressIcon;
        
        [Header ("Confirm Email"), SerializeField]
        private Transform _confirmEmailPanel;

        [SerializeField]
        private TMP_InputField _codeInput;
        
        [SerializeField]
        private Button _confirmButton;
        
        [SerializeField]
        private ProgressIcon _confirmProgressIcon;

        [Header ("Common"), SerializeField]
        private Button[] _signInButtons;

        [SerializeField]
        private float _fadeDuration;

        [Inject]
        private UILocker _uiLocker;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private AuthService _authService;

        [Inject]
        private SignUpValidator _signUpValidator;

        [Inject]
        private IMessageBroker _messageBroker;

        protected override void Awake ()
        {
            base.Awake ();

            _signUpButton
                .OnClickAsObservable ()
                .Subscribe (OnSignUpClicked)
                .AddTo (this);

            _confirmButton
                .OnClickAsObservable ()
                .Subscribe (OnConfirmClicked)
                .AddTo (this);
        }

        private void OnSignUpClicked (Unit _)
        {
            var email = _emailInput.text.Trim (' ');
            var password = _passwordInput.text.Trim (' ');
            var confirmPassword = _confirmPasswordInput.text.Trim (' ');

            if (!_signUpValidator.Validate (email, password, confirmPassword))
                return;

            async UniTaskVoid SignUp ()
            {
                _uiLocker.Lock (_signUpProgressIcon);
                await UniTask.Yield ();
                await _authService.SignUp (email, password);
                await _signUpPanel.FadeOut (_fadeDuration);
                await _confirmEmailPanel.FadeIn (_fadeDuration);
                _uiLocker.Unlock ();
            }

            SignUp ().Forget ();
        }

        private void OnConfirmClicked (Unit unit)
        {
            var email = _emailInput.text.Trim (' ');
            var password = _passwordInput.text.Trim (' ');
            var code = _codeInput.text.Trim (' ');

            async UniTaskVoid Verify ()
            {
                _uiLocker.Lock (_confirmProgressIcon);
                await UniTask.Yield ();
                await _authService.ConfirmSignUp (email, code);
                await _authService.SignIn (email, password);
                _messageBroker.Publish (new SignedInSignal ());
                _uiLocker.Unlock ();
            }

            Verify ().Forget ();
        }

        protected override void Refresh ()
        {
            _signUpPanel.FadeIn (true);
            _confirmEmailPanel.FadeOut (true);
        }

        protected override void Clear ()
        {
            _emailInput.text = string.Empty;
            _passwordInput.text = string.Empty;
            _confirmPasswordInput.text = string.Empty;
            _codeInput.text = string.Empty;
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _signInButtons
                .Select (x => x.OnClickAsObservable ())
                .Merge ()
                .Subscribe (_ => callbacks.OnSignIn ())
                .AddTo (_callbacksDisposables);
        }
    }
}