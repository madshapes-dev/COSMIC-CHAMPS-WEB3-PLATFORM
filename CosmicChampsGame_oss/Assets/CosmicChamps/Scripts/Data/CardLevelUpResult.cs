namespace CosmicChamps.Data
{
    public readonly struct CardLevelUpResult
    {
        public readonly int Level;
        public readonly PlayerCardShards CardShards;
        public readonly int UniversalShards;
        public readonly int PlayerLevel;
        public readonly int PlayerExp;

        public CardLevelUpResult (int level, PlayerCardShards cardShards, int universalShards, int playerLevel, int playerExp)
        {
            Level = level;
            CardShards = cardShards;
            UniversalShards = universalShards;
            PlayerLevel = playerLevel;
            PlayerExp = playerExp;
        }
    }
}