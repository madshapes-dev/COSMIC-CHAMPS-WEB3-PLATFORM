using System;
using System.Collections.Generic;
using System.Linq;
using CosmicChamps.Data;
using CosmicChamps.HomeScreen.Model;
using CosmicChamps.Services;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.HomeScreen.UI
{
    public class GamePresenter : AbstractPresenter<Unit, GamePresenter.Callbacks, GamePresenter>
    {
        private class DecksSnapshot
        {
            public readonly int ActiveDeckIndex;
            public readonly PlayerDeck[] DeckSnapshots;

            public DecksSnapshot (int activeDeckIndex, PlayerDeck[] deckSnapshots)
            {
                ActiveDeckIndex = activeDeckIndex;
                DeckSnapshots = deckSnapshots;
            }
        }

        public readonly struct Callbacks
        {
            public readonly Action<ProgressIcon> OnStartGame;
            public readonly Action<ProgressIcon> OnStartTournamentGame;

            public Callbacks (Action<ProgressIcon> onStartGame, Action<ProgressIcon> onStartTournamentGame)
            {
                OnStartGame = onStartGame;
                OnStartTournamentGame = onStartTournamentGame;
            }
        }

        [SerializeField]
        private TextMeshProUGUI _usernameCaption;

        [SerializeField]
        private Button _tournamentButton;

        [SerializeField]
        private Button _battleButton;

        [SerializeField]
        private Button _deckButton;

        [SerializeField]
        private Button _profileButton;

        [SerializeField]
        private Button _inventoryButton;

        [SerializeField]
        private Button _walletsButton;

        [SerializeField]
        private ProgressIcon _startTournamentBattleProgress;

        [SerializeField]
        private ProgressIcon _startBattleProgress;

        [SerializeField]
        private TextMeshProUGUI _ratingCaption;

        [SerializeField]
        private string _ratingCaptionPrefix;

        [SerializeField]
        private Progressbar _playerLevelProgressbar;

        [SerializeField]
        private TextMeshProUGUI _playerLevelCaption;

        [SerializeField]
        private TextMeshProUGUI _playerExpCaption;

        [SerializeField]
        private PlayerLevelUpPresenter _playerLevelUpPresenter;

        [Header ("Social"), SerializeField]
        private Button _xButton;

        [SerializeField]
        private Button _telegramButton;

        [SerializeField]
        private Button _discordButton;
        
        [SerializeField]
        private MissionsPresenter _missionsPresenter;

        [Inject]
        private ProfilePresenter _profilePresenter;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private DeckPresenter _deckPresenter;

        [Inject]
        private CardInfoPresenter _cardInfoPresenter;

        [Inject]
        private CompleteSignupPresenter _completeSignupPresenter;

        [Inject]
        private ILogger _logger;

        [Inject]
        private UILocker _uiLocker;

        [Inject]
        private IMessageBroker _messageBroker;

        [Inject]
        private Captions _captions;

        [Inject]
        private InventoryPresenter _inventoryPresenter;

        [Inject]
        private BattleRewardPresenter _battleRewardPresenter;

        [Inject]
        private WalletsPresenter _walletsPresenter;

        private DecksSnapshot _decksSnapshot;

        private void Start ()
        {
            _profileButton
                .OnClickAsObservable ()
                .Subscribe (_ => _profilePresenter.Display ())
                .AddTo (this);

            _deckButton
                .OnClickAsObservable ()
                .Subscribe (OnDeckButtonClicked)
                .AddTo (this);

            _inventoryButton
                .OnClickAsObservable ()
                .Subscribe (_ => OnInventoryClicked ())
                .AddTo (this);

            _walletsButton
                .OnClickAsObservable ()
                .Subscribe (_ => OnWalletsClicked ())
                .AddTo (this);

            _xButton
                .OnClickAsObservable ()
                .Subscribe (_ => Application.OpenURL (_gameService.GetCachedGameData ().SocialUrls.XUrl))
                .AddTo (this);
            _telegramButton
                .OnClickAsObservable ()
                .Subscribe (_ => Application.OpenURL (_gameService.GetCachedGameData ().SocialUrls.TelegramUrl))
                .AddTo (this);
            _discordButton
                .OnClickAsObservable ()
                .Subscribe (_ => Application.OpenURL (_gameService.GetCachedGameData ().SocialUrls.DiscordUrl))
                .AddTo (this);

            _deckPresenter.SetCallbacks (new DeckPresenter.Callbacks (OnCardInfoClicked));
            _deckPresenter
                .OnHiding
                .Subscribe (OnDecksHiding)
                .AddTo (this);

            _completeSignupPresenter.SetCallbacks (new CompleteSignupPresenter.Callbacks (OnCompleteSignupSubmitClicked));
        }

        private void OnWalletsClicked ()
        {
            _walletsPresenter.Display ();
        }

        private void OnInventoryClicked ()
        {
            _inventoryPresenter.Display ();
        }

        private void OnCompleteSignupSubmitClicked (string nickname, ProgressIcon progressIcon)
        {
            async UniTaskVoid CompleteSignUp ()
            {
                _uiLocker.Lock (progressIcon);
                await _gameService.CompleteSignUp (nickname);
                _uiLocker.Unlock ();

                var player = _gameService.GetCachedPlayer ();
                player.Nickname.Value = nickname;
                player.SignupCompleted = true;

                await _completeSignupPresenter.HideAsync ();
            }

            if (string.IsNullOrEmpty (nickname))
            {
                _messageBroker.Publish (ErrorSignal.CreateNonReportable (_captions.Errors.EmptyNickname));
                return;
            }

            CompleteSignUp ().Forget ();
        }

        private void OnDeckButtonClicked (Unit _)
        {
            var player = _gameService.GetCachedPlayer ();

            _decksSnapshot = new DecksSnapshot (
                player.ActiveDeckIndex,
                Array.ConvertAll (player.Decks, x => x.Clone ()));

            _deckPresenter.Display ();
        }

        private void OnDecksHiding (PresenterVisibilityEventArgs _)
        {
            var player = _gameService.GetCachedPlayer ();
            var changedDecks = player
                .Decks
                .Where ((deck, i) => !deck.Equals (_decksSnapshot.DeckSnapshots[i]))
                .ToArray ();

            async UniTaskVoid PersistDecksChange (int activeDeckIndex, IEnumerable<PlayerDeck> deck) =>
                await _gameService.UpdateDecks (activeDeckIndex, deck);

            if (player.ActiveDeckIndex != _decksSnapshot.ActiveDeckIndex || changedDecks.Any ())
            {
                _logger.Information (
                    "PersistDeckChanges ActiveDeckIndex changed: {ActiveDeckIndexChanged}, changed decks count {ChangedDecksCount}",
                    player.ActiveDeckIndex != _decksSnapshot.ActiveDeckIndex,
                    changedDecks.Length);

                PersistDecksChange (player.ActiveDeckIndex, changedDecks).Forget ();
            } else
            {
                _logger.Information ("Skip PersistDeckChanges");
            }

            _decksSnapshot = null;
        }

        private void OnCardInfoClicked (CardPresenterModel cardPresenterModelOrSlot) =>
            _cardInfoPresenter.Display (cardPresenterModelOrSlot);

        private void ProcessBattleReward ()
        {
            async UniTaskVoid ClearBattleRewards () => await _gameService.ClearBattleRewards ();

            var player = _gameService.GetCachedPlayer ();
            if (player.BattleRewards.Count == 0)
            {
                ClearBattleRewards ().Forget ();
                return;
            }

            var reward = player.BattleRewards.Dequeue ();
            _battleRewardPresenter.Display (new BattleRewardPresenter.Model (reward));
        }

        private void OnBattleRewardPresenterHiding ()
        {
            async UniTaskVoid ProcessNextReward ()
            {
                await UniTask.Delay (500);
                ProcessBattleReward ();
            }

            ProcessNextReward ().Forget ();
        }

        protected override void Refresh ()
        {
            this
                .ObserveEveryValueChanged (_ => _gameService.GetCachedPlayer ())
                .Where (x => x != null)
                .Subscribe (
                    player =>
                    {
                        player
                            .Nickname
                            .Subscribe (x => _usernameCaption.text = x)
                            .AddTo (_modelDisposables);

                        player
                            .Level
                            /*.Subscribe (
                                x => _playerLevelCaption.AnimateTextChangeThroughFade (
                                    x == _gameService.GetCachedGameData ().PlayerProgressions.Length - 1
                                        ? _captions.PlayerMaxLevel
                                        : _captions.PlayerLevel (x)))*/
                            .Subscribe (x => _playerLevelCaption.AnimateTextChangeThroughFade (x.ToString ()))
                            .AddTo (_modelDisposables);

                        player
                            .Level
                            .Skip (1)
                            .Subscribe (_ => _playerLevelUpPresenter.Display ())
                            .AddTo (_modelDisposables);

                        player
                            .Exp
                            .Subscribe (
                                x =>
                                {
                                    var playerLevel = player.Level.Value;
                                    var playerProgression = _gameService.GetCachedGameData ().PlayerProgressions;
                                    var progressValue = playerLevel == playerProgression.Length - 1
                                        ? 1f
                                        : (float)player.Exp.Value /
                                          playerProgression[playerLevel].LevelUpCost;

                                    _playerExpCaption.AnimateTextChangeThroughFade (
                                        playerLevel == _gameService.GetCachedGameData ().PlayerProgressions.Length - 1
                                            ? _captions.MaxLevel
                                            : $"{player.Exp.Value}/{playerProgression[playerLevel].LevelUpCost}");

                                    _playerLevelProgressbar.SetValue (progressValue);
                                })
                            .AddTo (_modelDisposables);
                        
                        _missionsPresenter.model = player;

                        _ratingCaption.text = $"{_ratingCaptionPrefix}{player.Rating.ToString ()}";
                        _tournamentButton.SetVisible (!string.IsNullOrEmpty (player.TournamentId));

                        if (!player.SignupCompleted)
                            _completeSignupPresenter.Display ();
                    })
                .AddTo (_modelDisposables);

            _battleRewardPresenter
                .OnHiding
                .Subscribe (_ => OnBattleRewardPresenterHiding ())
                .AddTo (_modelDisposables);

            ProcessBattleReward ();
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);

            _battleButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnStartGame (_startBattleProgress))
                .AddTo (_callbacksDisposables);

            _tournamentButton
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnStartTournamentGame (_startTournamentBattleProgress))
                .AddTo (_callbacksDisposables);
        }
    }
}