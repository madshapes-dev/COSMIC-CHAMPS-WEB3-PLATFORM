using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

public class PlayerShipCard
{
    [DynamoDBProperty]
    public string Id { set; get; }
    
    [DynamoDBProperty]
    public string[] Skins { set; get; }
}