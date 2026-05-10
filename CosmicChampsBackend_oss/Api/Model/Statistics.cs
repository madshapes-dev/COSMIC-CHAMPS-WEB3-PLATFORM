using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

[DynamoDBTable (nameof (Statistics))]
public class Statistics
{
    public const int DefaultId = 0;

    [DynamoDBHashKey]
    public int Id { set; get; }

    [DynamoDBProperty]
    public int GamesPlayed { set; get; }

    [DynamoDBProperty]
    public string? LastGameDate { set; get; }

    [DynamoDBProperty]
    public int TodayGamesPlayed { set; get; }
    
    [DynamoDBProperty]
    public int GamesBeforePrizeBot { set; get; }
}