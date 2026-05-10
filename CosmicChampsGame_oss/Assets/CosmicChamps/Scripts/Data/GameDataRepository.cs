using System;
using CosmicChamps.Api.Services;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Data
{
    public class GameDataRepository
    {
        private readonly Game.GameClient _gameClient;
        private GameData _gameDataCache;
        private Player _playerCache;

        public GameDataRepository (Game.GameClient gameClient)
        {
            _gameClient = gameClient;
        }

        public UniTask<Player> LoadPlayerData () => _gameClient
            .GetPlayerDataAsync (new PlayerDataRequest ())
            .ResponseAsync
            .ContinueWith (x => _playerCache = x.Result.PlayerData.ToModel ())
            .AsUniTask ();

        public UniTask<GameData> LoadGameData () => _gameClient
            .GetGameDataAsync (new GameDataRequest ())
            .ResponseAsync
            .ContinueWith (x => _gameDataCache = x.Result.GameData.ToModel ())
            .AsUniTask ();

        public UniTask<Player> UnbindAlgorandWallet () =>
            _gameClient
                .UnbindAlgorandWalletAsync (new UnbindWalletRequest ())
                .ResponseAsync
                .ContinueWith (x => _playerCache = x.Result.Player.ToModel ())
                .AsUniTask ();

        public UniTask<Player> BindImmutableWallet (string token) =>
            _gameClient
                .BindImmutableWalletAsync (
                    new BindImmutableWalletRequest
                    {
                        ImmutableToken = token
                    })
                .ResponseAsync
                .ContinueWith (x => _playerCache = x.Result.Player.ToModel ())
                .AsUniTask ();

        public UniTask<Player> UnbindImmutableWallet () =>
            _gameClient
                .UnbindImmutableWalletAsync (new UnbindWalletRequest ())
                .ResponseAsync
                .ContinueWith (x => _playerCache = x.Result.Player.ToModel ())
                .AsUniTask ();

        public async UniTask GuestBindEmail (string deviceId, string email) =>
            await _gameClient
                .BindEmailToGuestAsync (
                    new BindEmailToGuestRequest
                    {
                        DeviceId = deviceId,
                        Email = email
                    });

        public async UniTask ConfirmGuestBindEmail (string deviceId, string email, string password, string code, string token) =>
            await _gameClient
                .ConfirmBindEmailToGuestAsync (
                    new ConfirmBindEmailToGuestRequest
                    {
                        DeviceId = deviceId,
                        Email = email,
                        Password = password,
                        Code = code,
                        Token = token
                    })
                .ResponseAsync
                .ContinueWith (x => _playerCache = x.Result.PlayerData.ToModel ())
                .AsUniTask ();

        public void InvalidateCache ()
        {
            _gameDataCache = null;
            _playerCache = null;
        }

        public async UniTask<string> RedeemPromoCode (string promoCode)
        {
            var result = await _gameClient
                .RedeemPromoCodeAsync (new RedeemPromoCodeRequest { PromoCode = promoCode })
                .ResponseAsync;

            _playerCache = result.Player.ToModel ();
            return result.Message;
        }

        public GameData GetCachedGameData () => _gameDataCache;
        public Player GetCachedPlayer () => _playerCache;
    }
}