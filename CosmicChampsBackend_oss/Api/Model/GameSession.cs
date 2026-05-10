using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

[DynamoDBTable (nameof (GameSession))]
public class GameSession
{
    [DynamoDBHashKey]
    public string Id { set; get; }

    [DynamoDBProperty]
    public string IpAddress { set; get; }

    [DynamoDBProperty]
    public int Port { set; get; }

    [DynamoDBProperty]
    public string DnsName { set; get; }

    [DynamoDBProperty]
    public HashSet<string> PlayerIds { set; get; }
    
    [DynamoDBProperty]
    public string? TournamentId { set; get; }
    
    [DynamoDBProperty]
    public string? StartDate { set; get; }
}