using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model.DynamoDB;

public class MatchReportRepository : IMatchReportRepository
{
    private readonly DynamoDBContext _dynamoDbContext;

    public MatchReportRepository (DynamoDBContext dynamoDbContext)
    {
        _dynamoDbContext = dynamoDbContext;
    }

    public async Task CreateAsync (MatchReport matchReport)
    {
        await _dynamoDbContext.SaveAsync (matchReport);
    }
}