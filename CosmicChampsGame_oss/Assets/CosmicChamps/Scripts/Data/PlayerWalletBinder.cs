using System;
using CosmicChamps.Services;
using Cysharp.Threading.Tasks;
using UniRx;
using Zenject;

namespace CosmicChamps.Data
{
    /*public class PlayerWalletBinder : IInitializable, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly PlayerService _playerService;
        private readonly IGameService _gameService;
        private readonly IWalletService _walletService;

        public PlayerWalletBinder (PlayerService playerService, IGameService gameService, IWalletService walletService)
        {
            _playerService = playerService;
            _gameService = gameService;
            _walletService = walletService;
        }

        private async UniTaskVoid RefreshPlayerData ()
        {
            var isSignedIn = await _playerService.IsSignedIn ();
            if (!isSignedIn)
                return;

            await _gameService.LoadPlayerData ();
        }

        private void OnWalletConnected (string _)
        {
            RefreshPlayerData ().Forget ();
        }

        private void OnWalletDisconnected (Unit _)
        {
            RefreshPlayerData ().Forget ();
        }

        public void Initialize ()
        {
            _walletService
                .OnConnected
                .Subscribe (OnWalletConnected, _ => { })
                .AddTo (_disposables);

            _walletService
                .OnDisconnected
                .Subscribe (OnWalletDisconnected)
                .AddTo (_disposables);
        }

        public void Dispose ()
        {
            _disposables.Dispose ();
        }
    }*/
}