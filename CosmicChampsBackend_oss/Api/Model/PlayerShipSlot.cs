using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

public class PlayerShipSlot
{
    [DynamoDBProperty]
    public string Id { set; get; }
    
    [DynamoDBProperty]
    public string Skin { set; get; }
}