using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Common.Model;

[DynamoDBTable (nameof (ImmutableCredentials))]
public class ImmutableCredentials
{
    [DynamoDBHashKey]
    public string ImmutableId { set; get; }
    
    [DynamoDBProperty]
    public string PlayerId { set; get; }
    
    [DynamoDBProperty]
    public string WalletId { set; get; }
    
    [DynamoDBProperty]
    public string Email { set; get; }

    [DynamoDBProperty]
    public string Password { set; get; }    
}