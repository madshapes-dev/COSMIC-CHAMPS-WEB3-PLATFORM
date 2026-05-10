using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

public class PlayerCardShards
{
    [DynamoDBProperty]
    public string Id { set; get; }

    [DynamoDBProperty]
    public int Amount { set; get; }
}