using System;
using System.Collections.Generic;

namespace CosmicChamps.Data
{
    public class GameData
    {
        public Dictionary<string, UnitData> Units { set; get; }
        public Dictionary<string, CardData> Cards { set; get; }
        public Dictionary<string, ShipCardData> ShipCards { set; get; }
        public Dictionary<string, UnitData> BaseUnits { set; get; }
        public int InitialEnergy { set; get; }
        public int MaxEnergy { set; get; }
        public int MatchDuration { set; get; }
        public int OvertimeDuration { set; get; }
        public int MatchCountdown { set; get; }
        public int MatchCountdownDelay { set; get; }
        public int ForfeitPossibleDelay { set; get; }
        public EnergyGrowRate[] EnergyGrowRates { set; get; }
        public float ReconnectionTimeout { set; get; }
        public float UnitRepathInterval { set; get; }
        public int OpenedCardsCount => 4;
        public Bot[] Bots { set; get; }
        public DeckPreset[] DeckPresets { set; get; }
        public DeckPreset CustomDeckPreset { set; get; }
        public int AdvancedBotRatingThreshold { set; get; }
        public int EmojiDuration { set; get; }
        public CardProgression[] CardProgressions { set; get; }
        public PlayerProgression[] PlayerProgressions { set; get; }
        public string[] InventoryShardsOrder { set; get; }
        public string UniversalShardsId { set; get; }
        public SocialUrls SocialUrls { set; get; }
        public Missions Missions { set; get; }

        public UnitData GetUnit (string id)
        {
            if (!Units.TryGetValue (id, out var unitData))
                throw new ArgumentException ($"Invalid unit id {id}", nameof (id));

            return unitData;
        }

        public UnitData GetBaseUnit (string id)
        {
            if (!BaseUnits.TryGetValue (id, out var baseUnitData))
                throw new ArgumentException ($"Invalid base unit id {id}", nameof (id));

            return baseUnitData;
        }

        public CardData GetCard (string id)
        {
            if (!Cards.TryGetValue (id, out var cardData))
                throw new ArgumentException ($"Invalid card id {id}", nameof (id));

            return cardData;
        }

        public ShipCardData GetShipCard (string id)
        {
            if (!ShipCards.TryGetValue (id, out var shipCardData))
                throw new ArgumentException ($"Invalid ship card id {id}", nameof (id));

            return shipCardData;
        }
    }
}