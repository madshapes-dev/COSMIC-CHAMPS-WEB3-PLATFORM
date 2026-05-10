using System;
using CosmicChamps.Services;
using CosmicChamps.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class CompleteSignupPresenter : AbstractPresenter<Unit, CompleteSignupPresenter.Callbacks, CompleteSignupPresenter>
    {
        public readonly struct Callbacks
        {
            public readonly Action<string, ProgressIcon> OnSubmitClicked;

            public Callbacks (Action<string, ProgressIcon> onSubmitClicked)
            {
                OnSubmitClicked = onSubmitClicked;
            }
        }

        [SerializeField]
        private TMP_InputField _nicknameInput;

        [SerializeField]
        private Button _submitButton;

        [SerializeField]
        private ProgressIcon _progressIcon;

        [Inject]
        private IGameService _gameService;

        /*protected override void Refresh ()
        {
            base.Refresh ();
            _nicknameInput.text = _gameService.GetCachedPlayer ().Nickname.Value;
        }*/

        protected override void Clear ()
        {
            base.Clear ();
            _nicknameInput.text = null;
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _submitButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnSubmitClicked.Invoke (_nicknameInput.text, _progressIcon))
                .AddTo (_callbacksDisposables);
        }
    }
}