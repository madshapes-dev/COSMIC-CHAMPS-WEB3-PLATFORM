using System;
using CosmicChamps.Networking;
using CosmicChamps.Services;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using CosmicChamps.UI.PresentersGroups;
using Cysharp.Threading.Tasks;
using Serilog;
using UniRx;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class GamePresentersGroup : SinglePresentersGroup
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [Inject]
        private GamePresenter _gamePresenter;

        [Inject]
        private WaitingPresenter _waitingPresenter;

        [Inject]
        private IMessageBroker _messageBroker;

        [Inject]
        private IGameService _gameService;

        [Inject]
        private ClientNetworkService _networkService;

        [Inject]
        private SoundsService _soundsService;

        [Inject]
        private ILogger _logger;

        [Inject]
        private Captions _captions;

        protected override void Awake ()
        {
            base.Awake ();

            _messageBroker
                .Receive<SignedInSignal> ()
                .Subscribe (OnSignedInSignal)
                .AddTo (this);
        }

        private void Start ()
        {
            _gamePresenter.SetCallbacks (new GamePresenter.Callbacks (OnHomeScreenStartGame, OnHomeScreenStartTournamentGame));
        }

        protected override void OnDestroy ()
        {
            base.OnDestroy ();
            _soundsService.StopMusic ();
        }

        private void OnSignedInSignal (SignedInSignal _)
        {
            Display ();
        }

        private async UniTaskVoid StartGameAsync (bool tournament)
        {
            _waitingPresenter.SetCallbacks (new WaitingPresenter.Callbacks (null));
            await _waitingPresenter.DisplayAsync (new WaitingPresenter.Model ("Preparing for matchmaking"));

            try
            {
                var enumerator = _gameService.StartMatchmaking (tournament).GetAsyncEnumerator ();
                while (await enumerator.MoveNextAsync ())
                {
                    var matchMakingResult = enumerator.Current;
                    matchMakingResult.Switch (
                        playerGameSession =>
                        {
                            _logger.Information ("matchMakingResult");
                            _waitingPresenter.model = new WaitingPresenter.Model ("Opponent found");
                            _waitingPresenter.SetCallbacks (new WaitingPresenter.Callbacks (null));
                            _networkService.Start (playerGameSession);
                        },
                        matchmakingTicket =>
                        {
                            _logger.Information ("matchmakingTicket");
                            _waitingPresenter.model = new WaitingPresenter.Model ("Searching for an opponent");
                            _waitingPresenter.SetCallbacks (
                                new WaitingPresenter.Callbacks (
                                    () =>
                                    {
                                        async UniTaskVoid StopMatchmaking () =>
                                            await _gameService.StopMatchmaking (matchmakingTicket.Id);

                                        StopMatchmaking ().Forget ();
                                        _networkService.Stop ();
                                        _gamePresenter.Display ();
                                    }));
                        },
                        _ =>
                        {
                            _logger.Information ("matchmakingTimeout");
                            _messageBroker.Publish (
                                new ErrorSignal (
                                    "No match found. Please try again.",
                                    string.Empty,
                                    false,
                                    false,
                                    false));
                            _gamePresenter.Display ();
                        },
                        _ => _logger.Information ("matchmakingCancellation"));
                }
            } catch (Exception)
            {
                await _gamePresenter.DisplayAsync ();
                throw;
            }
        }

        private void OnHomeScreenStartGame (ProgressIcon progressIcon)
        {
            if (string.IsNullOrEmpty (_gameService.GetCachedPlayer ().WalletId))
            {
                _messageBroker.Publish (ErrorSignal.CreateNonReportable (_captions.Errors.WalletNotConnected));
                return;
            }

            StartGameAsync (false).Forget ();
        }

        private void OnHomeScreenStartTournamentGame (ProgressIcon progressIcon)
        {
            StartGameAsync (true).Forget ();
        }

        public override async UniTask DisplayAsync (PresenterDisplayOptions options = PresenterDisplayOptions.Notify)
        {
            _soundsService.PlayHomeScreenMusic ();

            _logger.Information ("LoadGameData");
            await _gameService.LoadGameData ();
            _logger.Information ("LoadPlayerData");
            await _gameService.LoadPlayerData ();
            _logger.Information ("DisplayAsync");
            await base.DisplayAsync (options);
        }

        public override UniTask HideAsync (PresenterDisplayOptions options = PresenterDisplayOptions.Default)
        {
            _soundsService.StopMusic ();
            return base.HideAsync (options);
        }
        #endif
    }
}