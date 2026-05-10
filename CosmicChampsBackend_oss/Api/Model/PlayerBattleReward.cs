using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

public class PlayerBattleReward
{
    [DynamoDBProperty]
    public PlayerCardShards[] Shards { set; get; }
    
    [DynamoDBProperty]
    public int OldRating { set; get; }
    
    [DynamoDBProperty]
    public int NewRating { set; get; }
    
    [DynamoDBProperty]
    public BattleResult Result { set; get; }
}