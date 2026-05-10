using System;
using System.Linq;
using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.UI;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.HomeScreen.UI
{
    public class DeckConfigurationDropdown : AbstractPresenter<
        PlayerDeck,
        DeckConfigurationDropdown.Callbacks,
        DeckConfigurationDropdown>
    {
        public readonly struct Callbacks
        {
            public readonly Action<DeckPreset> OnPresetChanged;

            public Callbacks (Action<DeckPreset> onPresetChanged)
            {
                OnPresetChanged = onPresetChanged;
            }
        }

        [SerializeField]
        private TMP_Dropdown _dropdown;

        [SerializeField]
        private TableReference _tableReference;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [Inject]
        private IGameService _gameService;

        [Inject]
        private ILogger _logger;

        private string GetPresetCaption (DeckPreset deckPreset) =>
            new LocalizedString (_tableReference, deckPreset.Name).GetLocalizedString ();

        private void OnPresetIdChanged (string presetId)
        {
            var gameData = _gameService.GetCachedGameData ();
            if (presetId == gameData.CustomDeckPreset.Id)
            {
                if (_dropdown.options.Count == gameData.DeckPresets.Length)
                    _dropdown.options.Add (new TMP_Dropdown.OptionData (GetPresetCaption (gameData.CustomDeckPreset)));

                _dropdown.SetValueWithoutNotify (gameData.DeckPresets.Length);
            } else
            {
                if (_dropdown.options.Count > gameData.DeckPresets.Length)
                    _dropdown.options.RemoveAt (_dropdown.options.Count - 1);

                _dropdown.SetValueWithoutNotify (gameData.DeckPresets.IndexOf (x => x.Id == presetId));
            }
        }

        protected override void Refresh ()
        {
            base.Refresh ();

            model
                .ObserveEveryValueChanged (x => x.PresetId)
                .Subscribe (OnPresetIdChanged)
                .AddTo (_modelDisposables);

            if (_dropdown.options.Count > 0)
                return;

            _dropdown.options = _gameService
                .GetCachedGameData ()
                .DeckPresets
                .Select (x => new TMP_Dropdown.OptionData (GetPresetCaption (x)))
                .ToList ();
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            void OnDropdownValueChanged (int value)
            {
                var gameData = _gameService.GetCachedGameData ();
                callbacks
                    .OnPresetChanged
                    ?.Invoke (
                        value < gameData.DeckPresets.Length
                            ? gameData.DeckPresets[value]
                            : gameData.CustomDeckPreset);
            }

            _dropdown
                .onValueChanged
                .AsObservable ()
                .Subscribe (OnDropdownValueChanged)
                .AddTo (_callbacksDisposables);
        }
        #endif
    }
}