using System;
using System.Collections.Generic;
using UniRx;

namespace CosmicChamps.Data
{
    public class Player
    {
        public string Id { set; get; }
        public readonly StringReactiveProperty Nickname = new();
        public readonly IntReactiveProperty NicknameChangeCount = new();
        public string Email { set; get; }
        public string WalletId { set; get; }
        public string LinkedWalletId { set; get; }
        public string ImmutableWalletId { set; get; }
        public PlayerDeck[] Decks { set; get; }
        public Dictionary<string, PlayerUnit> Units { set; get; }
        public Dictionary<string, PlayerCard> Cards { set; get; }
        public int ActiveDeckIndex { set; get; }
        public PlayerDeck ActiveDeck => Decks[ActiveDeckIndex];
        public string HUDSkin { set; get; }
        public int Rating { set; get; }
        public Dictionary<string, UnitBoost> Boosts { set; get; }
        public string TournamentId { set; get; }
        public Dictionary<string, PlayerShipCard> ShipCards { set; get; }
        public PlayerShipSlotCard ShipSlot { set; get; }
        public bool SignupCompleted { set; get; }
        public string[] Emojis { set; get; }
        public int UniversalShards { set; get; }
        public Dictionary<string, PlayerCardShards> CardShards { set; get; }
        public Queue<PlayerBattleReward> BattleRewards { set; get; }
        public readonly IntReactiveProperty Level = new();
        public readonly IntReactiveProperty Exp = new();
        public int GamesPlayed { set; get; }
        public AccountType AccountType { set; get; }
        public int MissionGamesPlayed { set; get; }

        public string DisplayName => Nickname.Value;

        public PlayerDeck GetDeck (int index)
        {
            if (index < 0 || index >= Decks.Length)
                throw new InvalidOperationException ($"Invalid deck index {index}");

            return Decks[index];
        }

        public PlayerCard GetCard (string id)
        {
            if (!Cards.TryGetValue (id, out var card))
                throw new InvalidOperationException ($"Invalid card id {id}");

            return card;
        }

        public PlayerUnit GetUnit (string id)
        {
            if (!Units.TryGetValue (id, out var unit))
                throw new InvalidOperationException ($"Invalid unit id {id}");

            return unit;
        }

        public PlayerCardShards GetCardShards (string id)
        {
            return CardShards.TryGetValue (id, out var cardShards)
                ? cardShards
                : new PlayerCardShards
                {
                    Id = id,
                    Amount = 0
                };
        }

        public PlayerShipCard GetShipCard (string id)
        {
            if (!ShipCards.TryGetValue (id, out var card))
                throw new InvalidOperationException ($"Invalid ship card id {id}");

            return card;
        }

        public UnitBoost GetBoost (string unitBoostId) => Boosts.TryGetValue (unitBoostId, out var unitBoost) ? unitBoost : null;
    }
}