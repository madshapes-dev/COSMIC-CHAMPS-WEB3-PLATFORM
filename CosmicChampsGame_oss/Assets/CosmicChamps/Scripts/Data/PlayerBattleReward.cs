namespace CosmicChamps.Data
{
    public class PlayerBattleReward
    {
        public PlayerCardShards[] Shards { set; get; }
        public int OldRaring { set; get; }
        public int NewRaring { set; get; }
        public BattleResult BattleResult { set; get; }
    }
}