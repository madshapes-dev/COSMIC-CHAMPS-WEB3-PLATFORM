using System;
using CosmicChamps.UI;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CosmicChamps.HomeScreen.UI
{
    public class WaitingPresenter : AbstractPresenter<WaitingPresenter.Model, WaitingPresenter.Callbacks, WaitingPresenter>
    {
        public readonly struct Model
        {
            public readonly string Message;

            public Model (string message)
            {
                Message = message;
            }
        }

        public readonly struct Callbacks
        {
            public readonly Action OnCanceled;

            public Callbacks (Action onCanceled)
            {
                OnCanceled = onCanceled;
            }
        }

        [FormerlySerializedAs ("label")]
        [SerializeField]
        private TextMeshProUGUI _label;

        [FormerlySerializedAs ("progressIcon")]
        [SerializeField]
        private ProgressIcon _progressIcon;

        [SerializeField]
        private Button _cancelButton;

        protected override void Awake ()
        {
            base.Awake ();
            ForceRefresh ();
        }

        protected override void Refresh ()
        {
            _label.text = model.Message;
            _label
                .AnimateTextWithEndingDots (model.Message)
                .AddTo (_modelDisposables);

            _progressIcon.FadeIn ();
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _cancelButton.SetVisible (callbacks.OnCanceled != null);
            _cancelButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnCanceled?.Invoke ())
                .AddTo (_callbacksDisposables);
        }

        protected override void Clear ()
        {
            base.Clear ();
            _progressIcon.FadeOut ();
        }
    }
}