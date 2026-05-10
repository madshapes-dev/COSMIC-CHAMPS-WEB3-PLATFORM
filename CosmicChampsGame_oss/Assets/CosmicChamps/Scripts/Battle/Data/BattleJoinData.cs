namespace CosmicChamps.Battle.Data
{
    public class BattleJoinData
    {
        public int InitialEnergy;
        public float EnergyGrowRate;
        public float Duration;
        public int MatchDuration;
        public float? OvertimeDuration;
        public bool ForfeitPossible;
        public string OpponentId;
        public string OpponentName;
        public int OpponentRating;
        public int OpponentLevel;
        public string PlayerEmoji;
        public string OpponentEmoji;
    }
}