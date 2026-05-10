using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

[DynamoDBTable (nameof (News))]
public class News
{
    [DynamoDBHashKey]
    public string Id { set; get; }
    
    [DynamoDBProperty]
    public string Header { set; get; }
    
    [DynamoDBProperty]
    public string Text { set; get; }
    
    [DynamoDBProperty]
    public string? Link { set; get; }
}