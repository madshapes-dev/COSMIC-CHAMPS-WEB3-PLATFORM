using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

public class PlayerDeckCard
{
    [DynamoDBProperty]
    public string Id { set; get; }

    [DynamoDBProperty]
    public string UnitSkin { set; get; }
}