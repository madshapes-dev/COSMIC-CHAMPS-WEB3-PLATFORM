using System;
using CosmicChamps.Data;
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
    public class BindEmailPresenter : AbstractPresenter<BindEmailPresenter.Model, BindEmailPresenter.Callbacks,
        BindEmailPresenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action OnEmailBound;

            public Callbacks (Action onEmailBound)
            {
                OnEmailBound = onEmailBound;
            }
        }

        public readonly struct Model
        {
            public readonly AbstractPresenter BackPresenter;

            public Model (AbstractPresenter backPresenter)
            {
                BackPresenter = backPresenter;
            }
        }

        [Header ("Bind"), SerializeField]
        private Transform _bindPanel;

        [SerializeField]
        private TMP_InputField _emailInput;

        [SerializeField]
        private TMP_InputField _passwordInput;

        [SerializeField]
        private TMP_InputField _confirmPasswordInput;

        [SerializeField]
        private Button _bindButton;

        [SerializeField]
        private ProgressIcon _bindProgressIcon;

        [Header ("Confirm"), SerializeField]
        private Transform _confirmPanel;

        [SerializeField]
        private TMP_InputField _codeInput;

        [SerializeField]
        private Button _confirmButton;

        [SerializeField]
        private ProgressIcon _confirmProgressIcon;

        [Space, SerializeField]
        private Button _firstStepCloseButton;

        [SerializeField]
        private Button _secondStepCloseButton;

        [SerializeField]
        private float _fadeDuration = 0.2f;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private UILocker _uiLocker;

        [Inject]
        private SignUpValidator _signUpValidator;

        [Inject]
        private TokensRepository _tokensRepository;

        [Inject]
        private IDeviceIdProvider _deviceIdProvider;

        private Callbacks _callbacks;

        protected override void Awake ()
        {
            base.Awake ();

            _bindButton
                .OnClickAsObservable ()
                .Subscribe (OnBindClicked)
                .AddTo (this);

            _confirmButton
                .OnClickAsObservable ()
                .Subscribe (OnConfirmClicked)
                .AddTo (this);

            _firstStepCloseButton
                .OnClickAsObservable ()
                .Merge (_secondStepCloseButton.OnClickAsObservable ())
                .Subscribe (OnBackClicked)
                .AddTo (this);
        }

        private void OnBindClicked (Unit _)
        {
            var email = _emailInput.text.Trim (' ');
            var password = _passwordInput.text.Trim (' ');
            var confirmPassword = _confirmPasswordInput.text.Trim (' ');

            if (!_signUpValidator.Validate (email, password, confirmPassword))
                return;

            async UniTaskVoid BindEmail ()
            {
                _uiLocker.Lock (_bindProgressIcon);
                await UniTask.Yield ();
                await _gameService.GuestBindEmail (_deviceIdProvider.DeviceId, email);

                await _bindPanel.FadeOut (_fadeDuration);
                await _confirmPanel.FadeIn (_fadeDuration);
                
                _firstStepCloseButton.enabled = false;
                _secondStepCloseButton.enabled = true;

                _uiLocker.Unlock ();
            }

            BindEmail ().Forget ();
        }

        private void OnConfirmClicked (Unit unit)
        {
            var email = _emailInput.text.Trim (' ');
            var password = _passwordInput.text.Trim (' ');
            var code = _codeInput.text.Trim (' ');

            async UniTaskVoid ConfirmEmail ()
            {
                _uiLocker.Lock (_confirmProgressIcon);

                var tokens = await _tokensRepository.Get ();
                await UniTask.Yield ();
                await _gameService.ConfirmGuestBindEmail (
                    _deviceIdProvider.DeviceId,
                    email,
                    password,
                    code,
                    tokens.AccessToken);

                _callbacks.OnEmailBound.Invoke ();

                _uiLocker.Unlock ();
            }

            ConfirmEmail ().Forget ();
        }

        private void OnBackClicked (Unit _)
        {
            Hide ();
            model.BackPresenter?.Display ();
        }

        protected override void Refresh ()
        {
            base.Refresh ();

            _bindPanel.FadeIn (true);
            _confirmPanel.FadeOut (true);
            _firstStepCloseButton.enabled = true;
            _secondStepCloseButton.enabled = false;
        }

        protected override void Clear ()
        {
            base.Clear ();

            _emailInput.text = string.Empty;
            _passwordInput.text = string.Empty;
            _confirmPasswordInput.text = string.Empty;
            _codeInput.text = string.Empty;
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);
            _callbacks = callbacks;
        }
    }
}