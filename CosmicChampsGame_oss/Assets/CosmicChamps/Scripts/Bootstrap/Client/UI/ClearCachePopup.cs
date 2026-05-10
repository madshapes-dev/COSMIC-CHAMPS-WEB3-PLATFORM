using System;
using ThirdParty.Extensions.CanvasGroupFader;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.Bootstrap.Client.UI
{
    public class ClearCachePopup : MonoBehaviour
    {
        [SerializeField]
        private Button _yesButton;

        [SerializeField]
        private Button _noButton;

        [Inject]
        private IMessageBroker _messageBroker;

        private Action _yesCallback;
        private Action _noCallback;

        private void Awake ()
        {
            _yesButton
                .OnClickAsObservable ()
                .Subscribe (OnYesClicked)
                .AddTo (this);

            _noButton
                .OnClickAsObservable ()
                .Subscribe (OnNoClicked)
                .AddTo (this);

            this.FadeOut (true);
        }

        private void OnYesClicked (Unit _)
        {
            _yesCallback?.Invoke ();
        }

        private void OnNoClicked (Unit _)
        {
            _noCallback?.Invoke ();
        }

        public void Display (Action yesCallback, Action noCallback)
        {
            _yesCallback = yesCallback;
            _noCallback = noCallback;

            this.FadeIn ();
        }

        public void Hide ()
        {
            _yesButton = _noButton = null;
            this.FadeOut ();
        }
    }
}