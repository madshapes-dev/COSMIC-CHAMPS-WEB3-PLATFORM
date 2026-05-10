using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

public class PlayerCard
{
    [DynamoDBProperty]
    public string Id { set; get; }
    
    [DynamoDBProperty]
    public int Level { set; get; }    
}