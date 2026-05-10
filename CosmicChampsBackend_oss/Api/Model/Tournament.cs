using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

[DynamoDBTable (nameof (Tournament))]
public class Tournament
{
    [DynamoDBHashKey]
    public string Id { set; get; }

    [DynamoDBProperty]
    public List<string> Players { set; get; }
}