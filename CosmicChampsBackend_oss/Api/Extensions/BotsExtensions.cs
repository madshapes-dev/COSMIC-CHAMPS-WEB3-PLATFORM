using CosmicChamps.Api.Model;
using CosmicChamps.Api.Services;
using AccountType = CosmicChamps.Api.Model.AccountType;
using PlayerCard = CosmicChamps.Api.Model.PlayerCard;
using PlayerCardShards = CosmicChamps.Api.Model.PlayerCardShards;
using PlayerDeck = CosmicChamps.Api.Model.PlayerDeck;
using PlayerDeckCard = CosmicChamps.Api.Model.PlayerDeckCard;
using PlayerShipCard = CosmicChamps.Api.Model.PlayerShipCard;
using PlayerShipSlot = CosmicChamps.Api.Model.PlayerShipSlot;
using PlayerUnit = CosmicChamps.Api.Model.PlayerUnit;
using UnitBoost = CosmicChamps.Api.Model.UnitBoost;

namespace CosmicChamps.Api.Extensions;

public static class BotsExtensions
{
    public static HashSet<string> GetIds (this IEnumerable<Bot> bots) =>
        bots
            .SelectMany (x => x.Levels.Select (x => x.Id))
            .ToHashSet ();

    public static BotLevel? GetBotLevel (this IEnumerable<Bot> bots, string id) =>
        bots
            .SelectMany (x => x.Levels)
            .FirstOrDefault (x => x.Id == id);

    public static int GetRatingThreshold (this Bot bot) => bot.Levels.Min (x => x.RatingThreshold);

    public static Player ToPlayer (this BotLevel botLevel, GameData gameData) => new()
    {
        Id = botLevel.Id,
        WalletId = botLevel.WalletId,
        LinkedWalletId = botLevel.WalletId,
        Email = botLevel.Nickname.Replace (' ', '\0'),
        Nickname = botLevel.Nickname,
        Boosts = botLevel.Boosts.Select (
                x => new UnitBoost
                {
                    Id = x.Id,
                    Hp = x.HpValue,
                    Damage = x.DamageValue,
                    DeathDamage = x.DeathDamageValue
                })
            .ToArray (),
        Units = gameData.Units.Select (
                x => new PlayerUnit
                {
                    Id = x.Id,
                    Skins = x.Aliases.Prepend (x.Id).ToArray ()
                })
            .ToArray (),
        Cards = botLevel.Deck.Select (
                x => new PlayerCard
                {
                    Id = x.Id,
                    Level = x.Level
                })
            .ToArray (),
        Rating = botLevel.Rating,
        Decks = new[]
        {
            new PlayerDeck
            {
                PresetId = "custom",
                Cards = botLevel.Deck.Select (
                        x => new PlayerDeckCard
                        {
                            Id = x.Id,
                            UnitSkin = x.Skin
                        })
                    .ToArray ()
            }
        },
        ActiveDeckIndex = 0,
        ShipCards = new[]
        {
            new PlayerShipCard
            {
                Id = "standard_ship",
                Skins = new[] { "default" }
            }
        },
        ShipSlot = new PlayerShipSlot
        {
            Id = "standard_ship",
            Skin = "default"
        },
        SignUpCompleted = true,
        Emojis = new[]
        {
            "ram", "coy", "inv"
        },
        AccountType = AccountType.Bot,
        UniversalShards = 0,
        CardShards = Array.Empty<PlayerCardShards> (),
        Level = botLevel.Level,
        Exp = 0
    };
}