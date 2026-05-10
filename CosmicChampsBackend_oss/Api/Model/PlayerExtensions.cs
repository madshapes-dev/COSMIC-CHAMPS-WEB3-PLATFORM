using Proto = CosmicChamps.Api.Services;

namespace CosmicChamps.Api.Model;

public static class PlayerExtensions
{
    public static void ValidateDecks (this Player player, Proto.GameData gameData)
    {
        if (player.Decks == null)
            return;

        for (var i = 0; i < player.Decks.Length; i++)
        {
            player.ValidateDeck (i, gameData);
        }
    }

    public static void ValidateDeck (this Player player, int deckIndex, Proto.GameData gameData)
    {
        if (player.Decks == null)
            return;

        if (deckIndex < 0 || deckIndex >= player.Decks.Length)
            throw new ArgumentException ("Invalid deck index");

        var deck = player.Decks[deckIndex];
        for (var i = 0; i < deck.Cards.Length; i++)
        {
            var card = deck.Cards[i];
            if (card == null)
                continue;

            var cardData = gameData.Cards.FirstOrDefault (x => x.Id == card.Id);
            if (cardData == null)
            {
                deck.Cards[i] = null;
                continue;
            }

            var playerCard = player.Cards.FirstOrDefault (x => x.Id == card.Id);
            if (playerCard == null)
            {
                deck.Cards[i] = null;
                continue;
            }

            var playerUnit = player.Units.FirstOrDefault (x => x.Id == cardData.UnitId);
            if (playerUnit == null)
            {
                deck.Cards[i] = null;
                continue;
            }
            
            if (cardData.LevelLock > player.Level && playerUnit.Skins.Length == 1)
            {
                deck.Cards[i] = null;
                continue;
            }

            if (!playerUnit.Skins.Contains (card.UnitSkin))
                card.UnitSkin = playerUnit.Skins.First ();
        }
    }

    public static void ResetToNoWallet (this Player player, Proto.GameData gameData)
    {
        player.Units = gameData.Units.Where (x => !x.Disabled)
            .Select (
                x => new PlayerUnit
                {
                    Id = x.Id,
                    Skins = new[] { x.Id }
                })
            .ToArray ();

        player.Boosts = Array.Empty<UnitBoost> ();
        player.HUDSkin = string.Empty;
        player.ValidateDecks (gameData);
    }
    
    public static void GrantShards(this Player player, string shardId, int amount)
    {
        var playerCardShards = player.CardShards.FirstOrDefault(x => x.Id == shardId);
        if (playerCardShards == null)
        {
            playerCardShards = new PlayerCardShards
            {
                Id = shardId,
                Amount = 0
            };

            player.CardShards = player.CardShards.Append(playerCardShards).ToArray();
        }

        playerCardShards.Amount += amount;        
    }
}