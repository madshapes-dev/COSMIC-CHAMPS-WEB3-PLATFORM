using System;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicChamps.Bootstrap.Client.UI
{
    public class UpdateRequiredPopup : AbstractPresenter<UpdateRequiredSignal, UpdateRequiredPopup.Callbacks, UpdateRequiredPopup>
    {
        public readonly struct Callbacks
        {
            public readonly Action<UpdateRequiredSignal> OnOkClicked;
            public readonly Action<UpdateRequiredSignal> OnContinueClicked;

            public Callbacks (Action<UpdateRequiredSignal> onOkClicked, Action<UpdateRequiredSignal> onContinueClicked)
            {
                OnOkClicked = onOkClicked;
                OnContinueClicked = onContinueClicked;
            }
        }

        [SerializeField]
        private TextMeshProUGUI _headerText;

        [SerializeField]
        private TextMeshProUGUI _messageText;

        [SerializeField]
        private string _softUpdateHeader = "Update Available";

        [SerializeField]
        private string _hardUpdateHeader = "Update Required";

        [SerializeField]
        private Button _okButton;

        [SerializeField]
        private Button _continueButton;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        protected override void Refresh ()
        {
            _headerText.text = model.Soft ? _softUpdateHeader : _hardUpdateHeader;
            _messageText.text = model.Message;
            _continueButton.SetVisible (model.Soft);
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _okButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnOkClicked (model))
                .AddTo (_callbacksDisposables);

            _continueButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnContinueClicked (model))
                .AddTo (_callbacksDisposables);
        }
        #endif
    }
}