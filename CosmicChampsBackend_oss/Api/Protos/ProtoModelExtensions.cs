using CosmicChamps.Api.Model;
using Proto = CosmicChamps.Api.Services;

namespace CosmicChamps.Api.Protos;

public static class ProtoModelExtensions
{
    public static Proto.PlayerCard ToProto (this PlayerCard card) =>
        new()
        {
            Id = card.Id,
            Level = card.Level
        };

    public static Proto.PlayerUnit ToProto (this PlayerUnit unit) =>
        new()
        {
            Id = unit.Id,
            Skins = { unit.Skins }
        };

    public static Proto.PlayerCardShards ToProto (this PlayerCardShards shards) =>
        new()
        {
            Id = shards.Id,
            Amount = shards.Amount
        };

    public static Proto.PlayerDeckCardOptional ToProto (this PlayerDeckCard? deckCard)
    {
        var protoDeckCard = new Proto.PlayerDeckCardOptional ();

        if (deckCard != null)
            protoDeckCard.CardValue = new Proto.PlayerDeckCard
            {
                Id = deckCard.Id,
                Skin = deckCard.UnitSkin
            };

        return protoDeckCard;
    }

    public static Proto.PlayerDeck ToProto (this PlayerDeck deck) =>
        new()
        {
            PresetId = deck.PresetId,
            Cards = { deck.Cards.Select (x => x.ToProto ()) }
        };

    public static PlayerDeckCard ToModel (this Proto.PlayerDeckCard deckCard) =>
        new()
        {
            Id = deckCard.Id,
            UnitSkin = deckCard.Skin
        };

    public static PlayerDeckCard? ToModel (this Proto.PlayerDeckCardOptional protoDeckCard) =>
        protoDeckCard.CardCase switch
        {
            Proto.PlayerDeckCardOptional.CardOneofCase.None => null,
            Proto.PlayerDeckCardOptional.CardOneofCase.CardValue => protoDeckCard.CardValue.ToModel (),
            _ => throw new ArgumentOutOfRangeException ()
        };

    public static Proto.UnitBoost ToProto (this UnitBoost unitBoost)
    {
        var protoUnitBoost = new Proto.UnitBoost
        {
            Id = unitBoost.Id
        };

        if (unitBoost.Hp != 0)
            protoUnitBoost.HpValue = unitBoost.Hp;

        if (unitBoost.Damage != 0)
            protoUnitBoost.DamageValue = unitBoost.Damage;

        if (unitBoost.DeathDamage > 0)
            protoUnitBoost.DeathDamageValue = unitBoost.DeathDamage;

        return protoUnitBoost;
    }

    public static Proto.PlayerShipSlot ToProto (this PlayerShipSlot? shipSlot)
    {
        var protoShipSlot = new Proto.PlayerShipSlot ();
        if (shipSlot != null)
            protoShipSlot.CardValue = new Proto.PlayerShipSlotCard
            {
                Id = shipSlot.Id,
                Skin = shipSlot.Skin
            };

        return protoShipSlot;
    }

    public static Proto.PlayerShipCard ToProto (this PlayerShipCard shipCard) =>
        new()
        {
            Id = shipCard.Id,
            Skins = { shipCard.Skins }
        };

    public static Proto.PlayerBattleReward ToProto (this PlayerBattleReward battleReward) =>
        new()
        {
            Shards = { battleReward.Shards.Select (x => x.ToProto ()) },
            OldRating = battleReward.OldRating,
            NewRating = battleReward.NewRating,
            BattleResult = battleReward.Result switch
            {
                BattleResult.Victory => Proto.BattleResult.Victory,
                BattleResult.Drawn => Proto.BattleResult.Drawn,
                BattleResult.Defeat => Proto.BattleResult.Defeat,
                BattleResult.Mission => Proto.BattleResult.Mission,
                _ => throw new ArgumentOutOfRangeException ()
            }
        };

    public static Proto.AccountType ToProto (this AccountType accountType) =>
        accountType switch
        {
            AccountType.Standard => Proto.AccountType.Standard,
            AccountType.Guest => Proto.AccountType.Guest,
            AccountType.Bot => Proto.AccountType.Bot,
            AccountType.Immutable => Proto.AccountType.Immutable,
            _ => throw new ArgumentOutOfRangeException (nameof (accountType), accountType, null)
        };

    public static Proto.PlayerData ToProto (this Player player)
    {
        var protoPlayerData = new Proto.PlayerData
        {
            Id = player.Id,
            Nickname = player.Nickname ?? string.Empty,
            Cards = { player.Cards.Select (x => x.ToProto ()) },
            Units = { player.Units.Select (x => x.ToProto ()) },
            Decks = { player.Decks?.Select (x => x.ToProto ()) ?? Array.Empty<Proto.PlayerDeck> () },
            ActiveDeck = player.ActiveDeckIndex,
            WalletId = player.WalletId,
            LinkedWalletId = player.LinkedWalletId ?? string.Empty,
            HudSkin = player.HUDSkin ?? string.Empty,
            Rating = player.Rating,
            TournamentId = player.TournamentId ?? string.Empty,
            ShipSlot = player.ShipSlot.ToProto (),
            ShipCards = { player.ShipCards?.Select (x => x.ToProto ()) ?? Array.Empty<Proto.PlayerShipCard> () },
            NicknameChangeCount = player.NicknameChangeCount,
            SignUpCompleted = player.SignUpCompleted,
            Email = player.Email,
            Emojis = { player.Emojis },
            AccountType = player.AccountType.ToProto (),
            UniversalShards = player.UniversalShards,
            CardShards = { player.CardShards.Select (x => x.ToProto ()) },
            BattleRewards = { player.BattleRewards?.Select (x => x.ToProto ()) ?? ArraySegment<Proto.PlayerBattleReward>.Empty },
            Level = player.Level,
            Exp = player.Exp,
            GamesPlayed = player.GamesPlayed,
            ImmutableWalletId = player.ImmutableWalletId ?? string.Empty,
            MissionGamesPlayed = player.MissionGamesPlayed 
        };

        if (player.Boosts != null)
            protoPlayerData.Boosts.AddRange (player.Boosts.Select (x => x.ToProto ()));

        return protoPlayerData;
    }

    public static Proto.News ToProto (this News news) =>
        new()
        {
            Id = news.Id,
            Header = news.Header,
            Text = news.Text,
            Link = news.Link ?? string.Empty
        };
}