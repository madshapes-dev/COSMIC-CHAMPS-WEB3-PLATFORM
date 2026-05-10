using System.Collections.Generic;
using CosmicChamps.Data;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Services
{
    public class ServerGameService : IGameService
    {
        private readonly GameDataRepository _gameDataRepository;

        public ServerGameService (GameDataRepository gameDataRepository)
        {
            _gameDataRepository = gameDataRepository;
        }

        public UniTask<GameData> LoadGameData () => _gameDataRepository.LoadGameData ();
        public UniTask<Player> LoadPlayerData () => throw new System.NotImplementedException ();

        public IUniTaskAsyncEnumerable<StartGameResult> StartMatchmaking (bool tournament) =>
            throw new System.NotImplementedException ();

        public UniTask StopMatchmaking (string ticketId) => throw new System.NotImplementedException ();

        public UniTask UpdateDecks (int activeDeckIndex, IEnumerable<PlayerDeck> decks) =>
            throw new System.NotImplementedException ();

        public GameData GetCachedGameData () => _gameDataRepository.GetCachedGameData ();
        public Player GetCachedPlayer () => throw new System.NotImplementedException ();
        public UniTask CompleteSignUp (string nickname) => throw new System.NotImplementedException ();
        public UniTask<int> SaveProfile (string nickname) => throw new System.NotImplementedException ();
        public UniTask<Player> BindImmutableWallet (string token) => throw new System.NotImplementedException ();
        public UniTask<Player> UnbindAlgorandWallet () => throw new System.NotImplementedException ();

        public UniTask<Player> UnbindImmutableWallet () => throw new System.NotImplementedException ();

        public UniTask GuestBindEmail (string deviceId, string email) => throw new System.NotImplementedException ();

        public UniTask ConfirmGuestBindEmail (string deviceId, string email, string password, string code, string token) =>
            throw new System.NotImplementedException ();

        public UniTask<CardLevelUpResult> CardLevelUp (string cardId) => throw new System.NotImplementedException ();
        public UniTask ClearBattleRewards () => throw new System.NotImplementedException ();
        public UniTask<string> RedeemPromoCode (string promoCode) => throw new System.NotImplementedException ();
    }
}