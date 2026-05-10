using UnityEngine;

namespace CosmicChamps.Data
{
    public static class PlayerCardExtensions
    {
        public static bool SimpleLevelUpPossible (this PlayerCard playerCard, Player player, GameData gameData)
        {
            var cardData = gameData.Cards[playerCard.Id];
            var cardProgression = gameData.CardProgressions;
            if (playerCard.Level.Value == cardProgression.Length - 1)
                return false;
            
            var playerCardShards = player.GetCardShards (cardData.UpgradeShardId);
            var cardShardsCost = cardProgression[playerCard.Level.Value].LevelUpCost;

            return !(cardShardsCost > playerCardShards.Amount);
        }

        public static bool CombinedLevelUpPossible (this PlayerCard playerCard, Player player, GameData gameData)
        {
            var cardData = gameData.Cards[playerCard.Id];
            var cardProgression = gameData.CardProgressions;
            if (playerCard.Level.Value == cardProgression.Length - 1)
                return false;
            
            var playerCardShards = player.GetCardShards (cardData.UpgradeShardId);
            var cardShardsCost = cardProgression[playerCard.Level.Value].LevelUpCost;
            var universalShardsCost = Mathf.Max (0, cardShardsCost - playerCardShards.Amount);

            return !(cardShardsCost > playerCardShards.Amount && universalShardsCost > player.UniversalShards);
        }

        public static bool LevelUpCapReached (this PlayerCard playerCard, Player player, GameData gameData)
        {
            var playerProgression = gameData.PlayerProgressions[player.Level.Value];
            return playerCard.Level.Value >= playerProgression.CardLevelCap;
        }
    }
}