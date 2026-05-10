using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

public class PlayerDeck
{
    [DynamoDBProperty]
    public string PresetId { set; get; }
    
    [DynamoDBProperty]
    public PlayerDeckCard?[] Cards { set; get; }
}