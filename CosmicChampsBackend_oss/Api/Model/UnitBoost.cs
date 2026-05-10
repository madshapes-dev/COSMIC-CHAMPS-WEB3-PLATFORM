using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

public class UnitBoost
{
    [DynamoDBProperty]
    public string Id { set; get; }

    [DynamoDBProperty]
    public int Hp { set; get; }

    [DynamoDBProperty]
    public int Damage { set; get; }
    
    [DynamoDBProperty]
    public int DeathDamage { set; get; }
}