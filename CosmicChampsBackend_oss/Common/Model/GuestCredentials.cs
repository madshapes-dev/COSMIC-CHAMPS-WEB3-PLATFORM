using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Common.Model;

[DynamoDBTable (nameof (GuestCredentials))]
public class GuestCredentials
{
    [DynamoDBHashKey]
    public string DeviceId { set; get; }
    
    [DynamoDBProperty]
    public string PlayerId { set; get; }
    
    [DynamoDBProperty]
    public string Email { set; get; }

    [DynamoDBProperty]
    public string Password { set; get; }
}