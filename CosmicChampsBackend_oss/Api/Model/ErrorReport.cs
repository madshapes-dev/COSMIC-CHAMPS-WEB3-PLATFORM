using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

[DynamoDBTable (nameof (ErrorReport))]
public class ErrorReport
{
    [DynamoDBHashKey]
    public string Id { set; get; }

    [DynamoDBProperty]
    public string Version { set; get; }

    [DynamoDBProperty]
    public string Platform { set; get; }

    [DynamoDBProperty]
    public string Date { set; get; }

    [DynamoDBProperty]
    public string GameSession { set; get; }

    [DynamoDBProperty]
    public string Message { set; get; }

    [DynamoDBProperty]
    public string Stacktrace { set; get; }

    [DynamoDBProperty]
    public Device Device { set; get; }
}