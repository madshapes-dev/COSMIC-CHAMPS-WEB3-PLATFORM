using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

public class PlayerUnit
{
    [DynamoDBProperty]
    public string Id { set; get; }

    [DynamoDBProperty]
    public string[] Skins { set; get; }
}