using System;
using System.Linq;
using CosmicChamps.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicChamps.Battle.UI
{
    public class ForfeitPopup : AbstractPresenter<Unit, ForfeitPopup.Callbacks, ForfeitPopup>
    {
        public readonly struct Callbacks
        {
            public readonly Action OnForfeit;

            public Callbacks (Action onForfeit)
            {
                OnForfeit = onForfeit;
            }
        }

        [SerializeField]
        private Button _forfeitButton;

        [SerializeField]
        private Button[] _closeButtons;

        protected override void Awake ()
        {
            base.Awake ();
            _closeButtons
                .Select (x => x.OnClickAsObservable ())
                .Merge ()
                .Subscribe (_ => Hide ())
                .AddTo (this);
        }

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        protected override void Refresh ()
        {
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _forfeitButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnForfeit ())
                .AddTo (_callbacksDisposables);
        }
        #endif
    }
}