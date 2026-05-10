using System;
using System.Collections.Generic;
using System.Linq;
using CosmicChamps.Data;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Services
{
    public class ClientGameService : IGameService
    {
        private readonly GameRepository _gameRepository;
        private readonly GameDataRepository _gameDataRepository;

        public ClientGameService (
            GameRepository gameRepository,
            GameDataRepository gameDataRepository)
        {
            _gameRepository = gameRepository;
            _gameDataRepository = gameDataRepository;
        }

        public UniTask<GameData> LoadGameData () => _gameDataRepository.LoadGameData ();
        public UniTask<Player> LoadPlayerData () => _gameDataRepository.LoadPlayerData ();

        public IUniTaskAsyncEnumerable<StartGameResult> StartMatchmaking (bool tournament)
        {
            var player = GetCachedPlayer ();
            return _gameRepository.StartMatchmaking (tournament ? player.TournamentId : null);
        }

        public UniTask StopMatchmaking (string ticketId) =>
            _gameRepository.StopMatchmaking (ticketId);

        public UniTask UpdateDecks (int activeDeckIndex, IEnumerable<PlayerDeck> decks)
        {
            var player = GetCachedPlayer ();
            return _gameRepository.UpdateDecks (activeDeckIndex, decks.Select (x => (Array.IndexOf (player.Decks, x), x)));
        }

        public GameData GetCachedGameData () => _gameDataRepository.GetCachedGameData ();
        public Player GetCachedPlayer () => _gameDataRepository.GetCachedPlayer ();
        public UniTask CompleteSignUp (string nickname) => _gameRepository.CompleteSignUp (nickname);
        public UniTask<int> SaveProfile (string nickname) => _gameRepository.SaveProfile (nickname);
        public UniTask<Player> BindImmutableWallet (string token) => _gameDataRepository.BindImmutableWallet (token);
        public UniTask<Player> UnbindAlgorandWallet () => _gameDataRepository.UnbindAlgorandWallet ();

        public UniTask<Player> UnbindImmutableWallet () => _gameDataRepository.UnbindImmutableWallet ();

        public UniTask GuestBindEmail (string deviceId, string email) =>
            _gameDataRepository.GuestBindEmail (deviceId, email);

        public UniTask ConfirmGuestBindEmail (string deviceId, string email, string password, string code, string token) =>
            _gameDataRepository.ConfirmGuestBindEmail (deviceId, email, password, code, token);

        public UniTask<CardLevelUpResult> CardLevelUp (string cardId) => _gameRepository.CardLevelUp (cardId);
        public UniTask ClearBattleRewards () => _gameRepository.ClearBattleRewards ();
        public UniTask<string> RedeemPromoCode (string promoCode) => _gameDataRepository.RedeemPromoCode (promoCode);
    }
}