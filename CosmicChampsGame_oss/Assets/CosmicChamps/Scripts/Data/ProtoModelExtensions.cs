using System;
using System.Collections.Generic;
using System.Linq;
using CosmicChamps.Api.Services;
using Vector2 = UnityEngine.Vector2;

namespace CosmicChamps.Data
{
    public static class ProtoModelExtensions
    {
        private static readonly Vector2[] _defaultBatchOffsets = { Vector2.zero };

        public static CardSkinData ToModel (
            this Api.Services.CardSkinData protoCardSkinData,
            string fallbackDisplayName,
            string fallbackDescription,
            string fallbackRarityNumber,
            string fallbackRarity) =>
            //
            new()
            {
                Id = protoCardSkinData.Id,
                DisplayName = string.IsNullOrEmpty (protoCardSkinData.DisplayName)
                    ? fallbackDisplayName
                    : protoCardSkinData.DisplayName,
                Description = string.IsNullOrEmpty (protoCardSkinData.Description)
                    ? fallbackDescription
                    : protoCardSkinData.Description,
                RarityNumber = string.IsNullOrEmpty (protoCardSkinData.RarityNumber)
                    ? fallbackRarityNumber
                    : protoCardSkinData.RarityNumber,
                Rarity = string.IsNullOrEmpty (protoCardSkinData.Rarity)
                    ? fallbackRarity
                    : protoCardSkinData.Rarity
            };

        public static CardData ToModel (this Api.Services.CardData protoCardData)
        {
            var batchOffsets = _defaultBatchOffsets;

            if ((protoCardData.BatchOffsets?.Count ?? 0) > 0)
                batchOffsets = protoCardData
                    .BatchOffsets
                    .Select (z => new Vector2 (z.X, z.Y))
                    .ToArray ();

            return new CardData
            {
                Id = protoCardData.Id,
                Skins = protoCardData
                    .Skins
                    .Select (
                        x => x.ToModel (
                            protoCardData.DisplayName,
                            protoCardData.Description,
                            protoCardData.RarityNumber,
                            protoCardData.Rarity))
                    .ToDictionary (x => x.Id),
                Energy = protoCardData.Energy,
                BatchSize = protoCardData.BatchSize,
                BatchOffsets = batchOffsets,
                DisplayName = protoCardData.DisplayName,
                Description = protoCardData.Description,
                Disabled = protoCardData.Disabled,
                UpgradeShardId = protoCardData.UpgradeShardId,
                UnitId = protoCardData.UnitId,
                RarityNumber = protoCardData.RarityNumber,
                Rarity = protoCardData.Rarity,
                LevelLock = protoCardData.LevelLock
            };
        }

        public static Damage ToModel (this Api.Services.Damage protoDamage) =>
            new(
                protoDamage.SplashVictims.ToModel (),
                protoDamage.Value.ToArray (),
                protoDamage.Rate,
                protoDamage.Range,
                protoDamage.Triggers.ToModel ()
            );

        public static UnitViewParams ToModel (this Api.Services.UnitViewParams protoUnitViewData)
        {
            if (protoUnitViewData == null)
                return null;

            return new()
            {
                MoveAnimationSpeed = protoUnitViewData.MoveAnimationSpeed,
                TurnTargetDuration = protoUnitViewData.TurnTargetDuration,
                AttackDuration = protoUnitViewData.AttackDuration,
                DeployDuration = protoUnitViewData.DeployDuration,
                DamageDelay = protoUnitViewData.DamageDelay,
                DeathDuration = protoUnitViewData.DeathDuration,
                DeathDamageDelay = protoUnitViewData.DeathDamageDelay
            };
        }

        public static UnitTargetType ToModel (this IEnumerable<TargetType> targetTypes) =>
            targetTypes.Aggregate (
                UnitTargetType.None,
                (targetType, protoTargetType) => targetType |
                                                 protoTargetType switch
                                                 {
                                                     TargetType.Base => UnitTargetType.Base,
                                                     TargetType.Ground => UnitTargetType.Ground,
                                                     TargetType.Air => UnitTargetType.Air,
                                                     TargetType.Spell => UnitTargetType.Spell,
                                                     _ => throw new ArgumentOutOfRangeException (
                                                         nameof (protoTargetType),
                                                         protoTargetType,
                                                         null)
                                                 });

        public static UnitStats ToModel (this Api.Services.UnitStats protoUnitStats) => new()
        {
            Hp = new Hp (protoUnitStats.Hp.ToArray ()),
            Damage = protoUnitStats.DamageCase == Api.Services.UnitStats.DamageOneofCase.DamageValue
                ? protoUnitStats.DamageValue.ToModel ()
                : null,
            DeathDamage = protoUnitStats.DeathDamageCase == Api.Services.UnitStats.DeathDamageOneofCase.DeathDamageValue
                ? protoUnitStats.DeathDamageValue.ToModel ()
                : null,
            Speed = protoUnitStats.Speed,
            DetectRange = protoUnitStats.DetectRange
        };

        public static IEnumerable<UnitData> ToModel (this Api.Services.UnitData protoUnitData)
        {
            var unitData = new UnitData
            {
                Id = protoUnitData.Id,
                BoostId = protoUnitData.BoostId,
                Stats = protoUnitData.Stats?.ToModel () ?? UnitStats.Empty,
                ViewParams = protoUnitData.ViewParams.ToModel (),
                MovementType = protoUnitData.MovementType switch
                {
                    MovementType.Ground => UnitMovementType.Ground,
                    MovementType.Air => UnitMovementType.Air,
                    MovementType.Spell => UnitMovementType.Spell,
                    _ => throw new ArgumentOutOfRangeException ()
                },
                SpawnArea = protoUnitData.SpawnArea,
                Type = protoUnitData.Type
            };

            if ((protoUnitData.Aliases?.Count ?? 0) == 0)
                return new[] { unitData };

            var result = new List<UnitData> { unitData };
            result.AddRange (protoUnitData.Aliases.Select (x => unitData.Clone (x)));

            return result;
        }

        public static PlayerDeck ToModel (this Api.Services.PlayerDeck protoDeck) =>
            new()
            {
                PresetId = protoDeck.PresetId,
                Cards = protoDeck
                    .Cards
                    .Select (x => x.ToModel ())
                    .ToArray ()
            };

        public static EnergyGrowRate ToModel (this Api.Services.EnergyGrowRate protoEnergyGrowRate) =>
            new()
            {
                From = protoEnergyGrowRate.From,
                Rate = protoEnergyGrowRate.Rate,
                Message = protoEnergyGrowRate.Message
            };

        /*public static Bot ToModel (this Api.Services.Bot protoBot) =>
            new()
            {
                Id = protoBot.Id,
                PlayerId = protoBot.PlayerId,
                WalletId = protoBot.WalletId,
                Email = protoBot.Email,
                Nickname = protoBot.Nickname
            };*/

        public static DeckPreset ToModel (this Api.Services.DeckPreset protoDeckPreset) =>
            new()
            {
                Id = protoDeckPreset.Id,
                Name = protoDeckPreset.Name,
                Cards = protoDeckPreset.Cards.Select (
                        x => new DeckPresetCard
                        {
                            Id = x.Id,
                            Skin = x.Skin
                        })
                    .ToArray ()
            };

        public static ShipCardData ToModel (this Api.Services.ShipCardData protoShipCardData) =>
            new()
            {
                Id = protoShipCardData.Id,
                MainId = protoShipCardData.MainId,
                ShieldId = protoShipCardData.ShieldId,
                TurretId = protoShipCardData.TurretId,
                DisplayName = protoShipCardData.DisplayName,
                Description = protoShipCardData.Description
            };

        public static CardProgression ToModel (this Api.Services.CardProgression protoCardProgression) =>
            new()
            {
                CumulativeCost = protoCardProgression.CumulativeCost,
                LevelUpCost = protoCardProgression.LevelUpCost,
                CosgCost = protoCardProgression.CosgCost,
                PlayerExp = protoCardProgression.PlayerExp
            };

        public static PlayerProgression ToModel (this Api.Services.PlayerProgression protoPlayerProgression) =>
            new()
            {
                LevelUpCost = protoPlayerProgression.LevelUpCost,
                CardLevelCap = protoPlayerProgression.CardLevelCap
            };

        public static SocialUrls ToModel (this Api.Services.SocialUrls protoSocialUrls) => new()
        {
            XUrl = protoSocialUrls.XUrl,
            TelegramUrl = protoSocialUrls.TelegramUrl,
            DiscordUrl = protoSocialUrls.DiscordUrl
        };

        public static Mission ToModel(this Api.Services.Mission promoMission) => new()
        {
            Title = promoMission.Title,
            GamesCount = promoMission.GamesCount,
            Reward = new MissionReward
            {
                Amount = promoMission.Reward.Amount,
                Kind = promoMission.Reward.Kind
            },
        };

        public static Missions ToModel(this Api.Services.Missions promoMissions) => new()
        {
            List = promoMissions.List.Select(x => x.ToModel()).ToArray(),
            Enabled = promoMissions.Enabled,
            Url = promoMissions.Url,
        };

        public static GameData ToModel (this Api.Services.GameData protoGameData) =>
            new()
            {
                Units = protoGameData.Units.SelectMany (y => y.ToModel ()).ToDictionary (y => y.Id),
                BaseUnits = protoGameData.BaseUnits.SelectMany (y => y.ToModel ()).ToDictionary (y => y.Id),
                Cards = protoGameData.Cards.Select (y => y.ToModel ()).ToDictionary (y => y.Id),
                ShipCards = protoGameData.ShipCards.Select (x => x.ToModel ()).ToDictionary (x => x.Id),
                InitialEnergy = protoGameData.InitialEnergy,
                MaxEnergy = protoGameData.MaxEnergy,
                MatchDuration = protoGameData.MatchDuration,
                MatchCountdown = protoGameData.MatchCountdown,
                MatchCountdownDelay = protoGameData.MatchCountdownDelay,
                ForfeitPossibleDelay = protoGameData.ForfeitPossibleDelay,
                OvertimeDuration = protoGameData.OvertimeDuration,
                EnergyGrowRates = protoGameData
                    .EnergyGrowRates
                    .Select (y => y.ToModel ())
                    .OrderBy (y => y.From)
                    .ToArray (),
                ReconnectionTimeout = protoGameData.ReconnectionTimeout,
                UnitRepathInterval = protoGameData.UnitRepathInterval,
                Bots = protoGameData
                    .Bots
                    .SelectMany (
                        x => x
                            .Levels
                            .Select (y => new Bot { Id = x.Id, PlayerId = y.Id }))
                    .ToArray (),
                DeckPresets = protoGameData.DeckPresets.Select (x => x.ToModel ()).ToArray (),
                CustomDeckPreset = protoGameData.CustomDeckPreset.ToModel (),
                AdvancedBotRatingThreshold = protoGameData.AdvancedBotRatingThreshold,
                EmojiDuration = protoGameData.EmojiDuration,
                CardProgressions = protoGameData.CardProgressions.Select (ToModel).ToArray (),
                PlayerProgressions = protoGameData.PlayerProgressions.Select (ToModel).ToArray (),
                InventoryShardsOrder = protoGameData.InventoryShardsOrder.ToArray (),
                UniversalShardsId = protoGameData.UniversalShardsId,
                SocialUrls = protoGameData.SocialUrls.ToModel (),
                Missions = protoGameData.Missions.ToModel()
            };

        public static PlayerDeckCard ToModel (this PlayerDeckCardOptional protoDeckCard) =>
            protoDeckCard.CardCase switch
            {
                PlayerDeckCardOptional.CardOneofCase.None => null,
                PlayerDeckCardOptional.CardOneofCase.CardValue => new()
                {
                    Id = protoDeckCard.CardValue.Id,
                    Skin = protoDeckCard.CardValue.Skin
                },
                _ => throw new ArgumentOutOfRangeException ()
            };

        public static PlayerCard ToModel (this Api.Services.PlayerCard protoCard) =>
            new()
            {
                Id = protoCard.Id,
                Level = { Value = protoCard.Level }
            };

        public static PlayerUnit ToModel (this Api.Services.PlayerUnit protoUnit) =>
            new()
            {
                Id = protoUnit.Id,
                Skins = protoUnit.Skins.ToArray ()
            };

        public static PlayerCardShards ToModel (this Api.Services.PlayerCardShards protoCardShards) => new()
        {
            Id = protoCardShards.Id,
            Amount = protoCardShards.Amount
        };

        public static UnitBoost ToModel (this Api.Services.UnitBoost protoUnitBoost) =>
            new()
            {
                Id = protoUnitBoost.Id,
                Hp = protoUnitBoost.HpCase == Api.Services.UnitBoost.HpOneofCase.HpValue
                    ? protoUnitBoost.HpValue
                    : null,
                Damage = protoUnitBoost.DamageCase == Api.Services.UnitBoost.DamageOneofCase.DamageValue
                    ? protoUnitBoost.DamageValue
                    : null,
                DeathDamage = protoUnitBoost.DeathDamageCase == Api.Services.UnitBoost.DeathDamageOneofCase.DeathDamageValue
                    ? protoUnitBoost.DeathDamageValue
                    : null
            };

        public static PlayerBattleReward ToModel (this Api.Services.PlayerBattleReward protoBattleReward) =>
            new()
            {
                Shards = protoBattleReward.Shards.Select (x => x.ToModel ()).ToArray (),
                OldRaring = protoBattleReward.OldRating,
                NewRaring = protoBattleReward.NewRating,
                BattleResult = protoBattleReward.BattleResult switch
                {
                    Api.Services.BattleResult.Victory => BattleResult.Victory,
                    Api.Services.BattleResult.Drawn => BattleResult.Drawn,
                    Api.Services.BattleResult.Defeat => BattleResult.Defeat,
                    Api.Services.BattleResult.Mission => BattleResult.Mission,
                    _ => throw new ArgumentOutOfRangeException ()
                }
            };

        public static Player ToModel (this PlayerData protoPlayerData)
        {
            var player = new Player
            {
                Id = protoPlayerData.Id,
                WalletId = protoPlayerData.WalletId,
                LinkedWalletId = protoPlayerData.LinkedWalletId,
                Units = protoPlayerData.Units.Select (x => x.ToModel ()).ToDictionary (x => x.Id),
                Cards = protoPlayerData.Cards.Select (x => x.ToModel ()).ToDictionary (x => x.Id),
                Decks = protoPlayerData.Decks.Select (x => x.ToModel ()).ToArray (),
                ActiveDeckIndex = protoPlayerData.ActiveDeck,
                HUDSkin = protoPlayerData.HudSkin,
                Rating = protoPlayerData.Rating,
                Boosts = protoPlayerData.Boosts.Select (x => x.ToModel ()).ToDictionary (x => x.Id),
                TournamentId = protoPlayerData.TournamentId,
                ShipCards = protoPlayerData.ShipCards.Select (x => x.ToModel ()).ToDictionary (x => x.Id),
                ShipSlot = protoPlayerData.ShipSlot.ToModel (),
                SignupCompleted = protoPlayerData.SignUpCompleted,
                Nickname = { Value = protoPlayerData.Nickname },
                NicknameChangeCount = { Value = protoPlayerData.NicknameChangeCount },
                Email = protoPlayerData.Email,
                Emojis = protoPlayerData.Emojis.ToArray (),
                UniversalShards = protoPlayerData.UniversalShards,
                CardShards = protoPlayerData.CardShards.Select (x => x.ToModel ()).ToDictionary (x => x.Id),
                BattleRewards = new Queue<PlayerBattleReward> (protoPlayerData.BattleRewards.Select (x => x.ToModel ())),
                Level = { Value = protoPlayerData.Level },
                Exp = { Value = protoPlayerData.Exp },
                GamesPlayed = protoPlayerData.GamesPlayed,
                AccountType = protoPlayerData.AccountType switch
                {
                    Api.Services.AccountType.Standard => AccountType.Standard,
                    Api.Services.AccountType.Guest => AccountType.Guest,
                    Api.Services.AccountType.Bot => AccountType.Bot,
                    Api.Services.AccountType.Immutable => AccountType.Immutable,
                    _ => throw new ArgumentOutOfRangeException ()
                },
                ImmutableWalletId = protoPlayerData.ImmutableWalletId,
                MissionGamesPlayed = protoPlayerData.MissionGamesPlayed
            };

            return player;
        }

        public static News ToModel (this Api.Services.News protoNews) =>
            protoNews == null
                ? null
                : new News
                {
                    Id = protoNews.Id,
                    Header = protoNews.Header,
                    Text = protoNews.Text,
                    Link = protoNews.Link
                };

        public static PlayerShipCard ToModel (this Api.Services.PlayerShipCard protoShipCard) =>
            new()
            {
                Id = protoShipCard.Id,
                Skins = protoShipCard.Skins.ToArray ()
            };

        public static PlayerShipSlotCard ToModel (this PlayerShipSlot protoShipSlot) =>
            protoShipSlot.CardCase switch
            {
                PlayerShipSlot.CardOneofCase.None => null,
                PlayerShipSlot.CardOneofCase.CardValue => new PlayerShipSlotCard
                {
                    Id = protoShipSlot.CardValue.Id,
                    Skin = protoShipSlot.CardValue.Skin
                },
                _ => throw new ArgumentOutOfRangeException ()
            };

        public static PlayerDeckCardOptional ToProto (this PlayerDeckCard deckCard)
        {
            var protoDeckCard = new PlayerDeckCardOptional ();

            if (deckCard != null)
                protoDeckCard.CardValue = new Api.Services.PlayerDeckCard
                {
                    Id = deckCard.Id,
                    Skin = deckCard.Skin
                };

            return protoDeckCard;
        }

        public static DeckUpdate ToProtoDeckUpdate (this PlayerDeck deck, int deckIndex) =>
            new()
            {
                DeckIndex = deckIndex,
                PresetId = deck.PresetId,
                Cards = { deck.Cards.Select (x => x.ToProto ()) }
            };
    }
}