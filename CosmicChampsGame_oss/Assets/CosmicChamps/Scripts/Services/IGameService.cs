using System.Collections.Generic;
using CosmicChamps.Data;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Services
{
    public interface IGameService
    {
        UniTask<GameData> LoadGameData ();
        UniTask<Player> LoadPlayerData ();
        IUniTaskAsyncEnumerable<StartGameResult> StartMatchmaking (bool tournament);
        UniTask StopMatchmaking (string ticketId);
        UniTask UpdateDecks (int activeDeckIndex, IEnumerable<PlayerDeck> decks);
        GameData GetCachedGameData ();
        Player GetCachedPlayer ();
        UniTask CompleteSignUp (string nickname);
        UniTask<int> SaveProfile (string nickname);
        UniTask<Player> UnbindAlgorandWallet ();
        UniTask<Player> BindImmutableWallet (string token);
        UniTask<Player> UnbindImmutableWallet ();
        UniTask GuestBindEmail (string deviceId, string email);
        UniTask ConfirmGuestBindEmail (string deviceId, string email, string password, string code, string token);
        UniTask<CardLevelUpResult> CardLevelUp (string cardId);
        UniTask ClearBattleRewards ();
        UniTask<string> RedeemPromoCode (string promoCode);
    }
}