using System;
using System.Globalization;
using CosmicChamps.Battle.Data.Client;
using CosmicChamps.Common;
using CosmicChamps.Data;
using CosmicChamps.Networking;
using CosmicChamps.UI;
using CosmicChamps.Utils;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Zenject;
using Player = CosmicChamps.Battle.Data.Client.Player;

namespace CosmicChamps.Battle.UI
{
    public class HUDPresenter : AbstractPresenter<HUDPresenter.Model, HUDPresenter.Callbacks, HUDPresenter>
    {
        public readonly struct Model
        {
            public readonly Data.Client.Battle Battle;
            public readonly Player Player;
            public readonly Opponent Opponent;

            public Model (Data.Client.Battle battle, Player player, Opponent opponent)
            {
                Battle = battle;
                Player = player;
                Opponent = opponent;
            }
        }

        public readonly struct Callbacks
        {
            public readonly Action OnForfeit;

            public Callbacks (Action onForfeit)
            {
                OnForfeit = onForfeit;
            }
        }

        [SerializeField]
        private TextMeshProUGUI _opponentNameCaption;
        
        [SerializeField]
        private TextMeshProUGUI _opponentLevelCaption;

        [SerializeField]
        private TextMeshProUGUI _opponentRatingCaption;

        [SerializeField]
        private string _ratingPrefix;

        [SerializeField]
        private DeckPresenter _deckPresenter;

        [SerializeField]
        private EnergyBarPresenter _energyBarPresenter;

        [SerializeField]
        private TextMeshProUGUI _timerLabel;

        [SerializeField]
        private Button _forfeitButton;

        [SerializeField]
        private Button _copyDebugInfo;

        [SerializeField]
        private LocalizeStringEvent _timeLeftLocalizeEvent;

        [SerializeField]
        private LocalizedString _timeLeftStringReference;

        [SerializeField]
        private LocalizedString _overtimeStringReference;

        [Header ("Skinning"), SerializeField]
        private HUDSkins _hudSkins;

        [SerializeField]
        private Image _deckBackground;

        [SerializeField]
        private Image _timeBackground;

        [SerializeField]
        private Image _energyBackground;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [Inject]
        private ITimeProvider _timeProvider;

        [Inject]
        private ClientNetworkService _clientNetworkService;

        [Inject]
        private GameDataRepository _gameDataRepository;

        [Inject]
        private ForfeitPopup _forfeitPopup;

        [Inject]
        private IEmojisProvider _emojisProvider;

        [Inject]
        private Emojis _emojis;

        [Inject]
        private Captions _captions;

        private IDisposable _timerDisposable;

        private void OnForfeitClicked (Unit _)
        {
            _forfeitPopup.Display ();
        }

        private void RefreshTimerCaption (TimeSpan timespan)
        {
            _timerLabel.text = $"<mspace=0.7em>{timespan.Minutes}</mspace>:<mspace=0.7em>{timespan.Seconds:00}";
        }

        private void OnTimer (long _)
        {
            var battle = model.Battle;
            var seconds = battle.MatchDuration +
                          (battle.OvertimeDuration ?? 0f) -
                          (battle.Duration + _timeProvider.Time - battle.DurationTimestamp);
            RefreshTimerCaption (TimeSpan.FromSeconds (Mathf.Max (0f, seconds)));
            var overtime = battle.OvertimeDuration.HasValue;


            _timeLeftLocalizeEvent.StringReference = overtime
                ? _overtimeStringReference
                : _timeLeftStringReference;

            var player = _gameDataRepository.GetCachedPlayer ();
            var hudSkin = _hudSkins.GetSkin (player.HUDSkin);
            _timeBackground.sprite = overtime
                ? hudSkin.OvertimeTimePanelSprite
                : hudSkin.TimePanelSprite;
        }

        private void OnPlayerEmoji (string emoji) => _emojis.Player.Display (_emojisProvider, emoji);

        private void OnOpponentEmoji (string emoji) => _emojis.Opponent.Display (_emojisProvider, emoji);

        protected override void Refresh ()
        {
            _emojis.Player.Hide (true);
            _emojis.Opponent.Hide (true);

            _forfeitButton
                .OnClickAsObservable ()
                .Subscribe (OnForfeitClicked)
                .AddTo (_modelDisposables);

            _copyDebugInfo
                .OnClickAsObservable ()
                .Subscribe (OnCopyDebugInfoClicked)
                .AddTo (_modelDisposables);

            RefreshTimerCaption (TimeSpan.FromSeconds (model.Battle.MatchDuration));

            var battlePlayer = model.Player;
            _deckPresenter.model = new DeckPresenter.Model (battlePlayer.Deck);
            _energyBarPresenter.model = new EnergyBarPresenter.Model (battlePlayer.Energy);

            _opponentNameCaption.text = model.Opponent.Name;
            _opponentRatingCaption.text = $"{_ratingPrefix}{model.Opponent.Rating}";
            _opponentLevelCaption.text = _captions.PlayerLevel (model.Opponent.Level);

            var player = _gameDataRepository.GetCachedPlayer ();
            var hudSkin = _hudSkins.GetSkin (player.HUDSkin);

            _deckBackground.sprite = hudSkin.DeckPanelSprite;
            _energyBackground.sprite = hudSkin.EnergyBackground;

            var overtime = model.Battle.OvertimeDuration.HasValue;

            _timeLeftLocalizeEvent.StringReference = overtime
                ? _overtimeStringReference
                : _timeLeftStringReference;

            _timeBackground.sprite = overtime
                ? hudSkin.OvertimeTimePanelSprite
                : hudSkin.TimePanelSprite;

            model.Player.Emoji.Subscribe (OnPlayerEmoji).AddTo (_modelDisposables);
            model.Opponent.Emoji.Subscribe (OnOpponentEmoji).AddTo (_modelDisposables);
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);
            _callbacksDisposables.Add (
                new SetPresenterCallbacksCommand<ForfeitPopup, Unit, ForfeitPopup.Callbacks> (
                        _forfeitPopup,
                        new ForfeitPopup.Callbacks (callbacks.OnForfeit))
                    .Run ());
        }

        private void OnCopyDebugInfoClicked (Unit unit)
        {
            var gameSession = _clientNetworkService.PlayerGameSession;
            UniClipboard.SetText (
                $"Version: {BuildInfo.AppVersionString}\nPlatform: {Application.platform}\nDate: {DateTime.Now.ToString (CultureInfo.InvariantCulture)}\nGame Session: {gameSession.Id}\nPlayer Session Id: {gameSession.PlayerSessionId}");
        }

        protected override void Clear ()
        {
            base.Clear ();

            StopTimer ();
            _timerDisposable?.Dispose ();
            _timerDisposable = null;
        }

        public void StartTimer ()
        {
            _timerDisposable = Observable
                .Timer (TimeSpan.Zero, TimeSpan.FromSeconds (1f))
                .Subscribe (OnTimer)
                .AddTo (this);
        }

        public void SetForfeitPossible (bool enabled)
        {
            _forfeitButton.SetVisible (enabled);
        }

        public void StopTimer ()
        {
        }
        #endif
    }
}