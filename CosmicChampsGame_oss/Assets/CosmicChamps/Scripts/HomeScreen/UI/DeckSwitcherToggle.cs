using System;
using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.UI;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.HomeScreen.UI
{
    public class DeckSwitcherToggle : AbstractPoolablePresenter<PlayerDeck, DeckSwitcherToggle.Callbacks, DeckSwitcherToggle>
    {
        public readonly struct Callbacks
        {
            public readonly Action<PlayerDeck> OnSelected;

            public Callbacks (Action<PlayerDeck> onSelected)
            {
                OnSelected = onSelected;
            }
        }

        public class Factory : AbstractFactory
        {
        }

        [SerializeField]
        private TextMeshProUGUI _caption;

        [SerializeField]
        private Toggle _toggle;

        [SerializeField]
        private TMP_ColorGradient _onGradient;

        [SerializeField]
        private TMP_ColorGradient _offGradient;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [Inject]
        private IGameService _gameService;

        [Inject]
        private ILogger _logger;

        protected override void Awake ()
        {
            base.Awake ();

            _toggle
                .OnValueChangedAsObservable ()
                .Subscribe (OnValueChanged)
                .AddTo (this);
        }

        private void SetState ()
        {
            _caption.colorGradientPreset = _toggle.isOn ? _onGradient : _offGradient;
        }

        private void OnValueChanged (bool value)
        {
            SetState ();
        }

        protected override void Refresh ()
        {
            base.Refresh ();

            var player = _gameService.GetCachedPlayer ();
            var index = player.Decks.IndexOf (model) + 1;
            _caption.text = index.ToString ();

            _toggle.SetIsOnWithoutNotify (player.ActiveDeck.Equals (model));
            SetState ();
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _toggle
                .OnValueChangedAsObservable ()
                .Skip (1)
                .Where (x => x)
                .Subscribe (_ => callbacks.OnSelected?.Invoke (model))
                .AddTo (_callbacksDisposables);
        }

        public DeckSwitcherToggle SetToggleGroup (ToggleGroup group)
        {
            _toggle.group = group;
            return this;
        }
        #endif
    }
}