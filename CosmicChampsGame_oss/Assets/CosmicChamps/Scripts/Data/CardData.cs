using System;
using System.Collections.Generic;
using UnityEngine;

namespace CosmicChamps.Data
{
    public class CardData
    {
        public string Id { set; get; }
        public Dictionary<string, CardSkinData> Skins { set; get; }
        public int Energy { set; get; }
        public int BatchSize { set; get; }
        public Vector2[] BatchOffsets { set; get; }
        public string DisplayName { set; get; }
        public string Description { set; get; }
        public bool Disabled { set; get; }
        public string UpgradeShardId { set; get; }
        public string UnitId { set; get; }
        public string RarityNumber { set; get; }
        public string Rarity { set; get; }
        public int LevelLock { set; get; }

        public CardSkinData GetSkin (string id)
        {
            if (!Skins.TryGetValue (id, out var skin))
                throw new InvalidOperationException ($"Unable to get skin {id} for card {Id}");

            return skin;
        }
    }
}