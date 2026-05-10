using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using CosmicChamps.UI.PresentersGroups;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class SignPresentersGroup : SinglePresentersGroup
    {
        [Header ("Errors"), SerializeField]
        private LocalizedString _emptyEmailError;

        [SerializeField]
        private LocalizedString _invalidEmailError;

        [SerializeField]
        private LocalizedString _emptyPasswordError;

        [SerializeField]
        private LocalizedString _passwordNotMatchError;

        [SerializeField]
        private ProgressIcon _immutableProgressIcon;

        [Inject]
        private LandingPresenter _landingPresenter;

        [Inject]
        private SignInPresenter _signInPresenter;

        [Inject]
        private EmailSignInPresenter _emailSignInPresenter;

        [Inject]
        private SignUpPresenter _signUpPresenter;

        [Inject]
        private ResetPasswordStep1Presenter _resetPasswordStep1Presenter;

        [Inject]
        private ResetPasswordStep2Presenter _resetPasswordStep2Presenter;

        [Inject]
        private AuthService _authService;

        [Inject]
        private IMessageBroker _messageBroker;

        [Inject]
        private GameDataRepository _gameDataRepository;

        [Inject]
        private UILocker _uiLocker;

        [Inject]
        private SignUpValidator _signUpValidator;

        [Inject]
        private IDeviceIdProvider _deviceIdProvider;

        [Inject]
        private IImmutableService _immutableService;

        private readonly CompositeDisposable _disposables = new();

        protected override void Awake ()
        {
            base.Awake ();
            _messageBroker
                .Receive<SignedOutSignal> ()
                .Subscribe (OnSignedOutSignal)
                .AddTo (this);
        }

        private void Start ()
        {
            _landingPresenter.SetCallbacks (
                new LandingPresenter.Callbacks (
                    OnLandingEmailClicked,
                    OnLandingSignInClicked,
                    OnLandingGuestSignInClicked,
                    OnLandingImmutableClicked));

            _signInPresenter.SetCallbacks (
                new SignInPresenter.Callbacks (
                    OnSignInEmailClicked,
                    OnSignInImmutableClicked,
                    OnSignInSignUpClicked));

            _emailSignInPresenter.SetCallbacks (
                new EmailSignInPresenter.Callbacks (
                    OnEmailSignInSignInClicked,
                    OnEmailSignInSignUpClicked,
                    OnEmailSignInResetPasswordClicked));

            _signUpPresenter.SetCallbacks (new SignUpPresenter.Callbacks (OnSignUpSignInClicked));

            _resetPasswordStep1Presenter.SetCallbacks (
                new ResetPasswordStep1Presenter.Callbacks (ResetPasswordStep1ResetClicked, ResetPasswordBackClicked));
            _resetPasswordStep2Presenter.SetCallbacks (
                new ResetPasswordStep2Presenter.Callbacks (ResetPasswordStep2ResetClicked, ResetPasswordBackClicked));
        }

        private void OnLandingEmailClicked ()
        {
            _signUpPresenter.Display ();
        }

        private void OnLandingSignInClicked ()
        {
            _signInPresenter.Display ();
        }

        private void OnLandingGuestSignInClicked (ProgressIcon progressIcon)
        {
            GuestSignIn (progressIcon);
        }

        private void OnLandingImmutableClicked ()
        {
            ImmutableSignIn ();
        }

        private void OnSignInEmailClicked ()
        {
            _emailSignInPresenter.Display ();
        }

        private void OnSignInImmutableClicked ()
        {
            ImmutableSignIn ();
        }

        private void OnSignInSignUpClicked ()
        {
            _landingPresenter.Display ();
        }

        private void OnEmailSignInSignInClicked (string email, string password, ProgressIcon progressIcon)
        {
            async UniTaskVoid SignInAsync ()
            {
                _uiLocker.Lock (progressIcon);
                await UniTask.Yield ();
                await _authService.SignIn (email, password);
                _messageBroker.Publish (new SignedInSignal ());
                _uiLocker.Unlock ();
            }

            email = email.Trim (' ');
            password = password.Trim (' ');

            if (string.IsNullOrEmpty (email))
            {
                FireErrorSignal (_emptyEmailError.GetLocalizedString (), string.Empty);
                return;
            }

            if (!email.IsValidEmail ())
            {
                FireErrorSignal (_invalidEmailError.GetLocalizedString (), string.Empty);
                return;
            }

            if (string.IsNullOrEmpty (password))
            {
                FireErrorSignal (_emptyPasswordError.GetLocalizedString (), string.Empty);
                return;
            }

            SignInAsync ().Forget ();
        }

        private void OnEmailSignInSignUpClicked ()
        {
            _landingPresenter.Display ();
        }

        private void OnEmailSignInResetPasswordClicked (string arg1, ProgressIcon arg2)
        {
            _resetPasswordStep1Presenter.Display ();
        }

        private void OnSignUpSignInClicked ()
        {
            _signInPresenter.Display ();
        }

        private void OnSignedOutSignal (SignedOutSignal _)
        {
            _authService.SignOut ();
            _gameDataRepository.InvalidateCache ();

            Display ();
        }

        private void GuestSignIn (ProgressIcon progressIcon)
        {
            async UniTaskVoid GuestSignInAsync ()
            {
                _uiLocker.Lock (progressIcon);
                await UniTask.Yield ();
                await _authService.GuestSignIn (_deviceIdProvider.DeviceId);
                _messageBroker.Publish (new SignedInSignal ());
                _uiLocker.Unlock ();
            }

            GuestSignInAsync ().Forget ();
        }

        private void ImmutableSignIn ()
        {
            async UniTaskVoid ImmutableSignInAsync ()
            {
                _uiLocker.Lock (_immutableProgressIcon);
                await UniTask.Yield ();
                await _authService.ImmutableSignIn ();
                _messageBroker.Publish (new SignedInSignal ());
                _uiLocker.Unlock ();
            }

            ImmutableSignInAsync ().Forget ();
        }

        private void FireErrorSignal (string message, string stacktrace) =>
            _messageBroker.Publish (new ErrorSignal (message, stacktrace, false, false, false));

        private void ResetPasswordStep1ResetClicked (string email, ProgressIcon progressIcon)
        {
            email = email.Trim (' ');

            if (string.IsNullOrEmpty (email))
            {
                FireErrorSignal (_emptyEmailError.GetLocalizedString (), string.Empty);
                return;
            }

            if (!email.IsValidEmail ())
            {
                FireErrorSignal (_invalidEmailError.GetLocalizedString (), string.Empty);
                return;
            }

            async UniTaskVoid GetResetPasswordCode ()
            {
                _uiLocker.Lock (progressIcon);
                await UniTask.Yield ();
                var codeDestination = await _authService.GetResetPasswordCodeAsync (email);
                await _resetPasswordStep2Presenter.DisplayAsync (new ResetPasswordStep2Presenter.Model (email, codeDestination));
                _uiLocker.Unlock ();
            }

            GetResetPasswordCode ().Forget ();
        }

        private void ResetPasswordStep2ResetClicked (
            string email,
            string code,
            string password,
            string confirmPassword,
            ProgressIcon progressIcon)
        {
            password = password.Trim (' ');
            confirmPassword = confirmPassword.Trim (' ');

            if (!_signUpValidator.Validate (email, password, confirmPassword))
                return;

            async UniTaskVoid ResetPassword ()
            {
                _uiLocker.Lock (progressIcon);
                await UniTask.Yield ();
                var error = await _authService.ResetPassword (email, password, code);
                if (!string.IsNullOrEmpty (error))
                {
                    FireErrorSignal (error, string.Empty);
                    return;
                }

                await _emailSignInPresenter.DisplayAsync ();
                _uiLocker.Unlock ();
            }

            ResetPassword ().Forget ();
        }

        private void ResetPasswordBackClicked ()
        {
            _emailSignInPresenter.Display ();
        }

        protected override void OnDestroy ()
        {
            base.OnDestroy ();
            _disposables.Dispose ();
        }

        public override UniTask HideAsync (PresenterDisplayOptions options = PresenterDisplayOptions.Default)
        {
            _disposables.Clear ();
            return base.HideAsync (options);
        }
    }
}