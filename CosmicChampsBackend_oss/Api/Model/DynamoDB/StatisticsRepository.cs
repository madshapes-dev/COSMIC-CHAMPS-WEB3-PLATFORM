using Amazon.DynamoDBv2.DataModel;
using CosmicChamps.Api.Configs;
using CosmicChamps.Common.Configs;

namespace CosmicChamps.Api.Model.DynamoDB;

public class StatisticsRepository : IStatisticsRepository
{
    private readonly DynamoDBContext _dynamoDbContext;
    private readonly DynamoDBOperationConfig _dbOperationConfig;

    public StatisticsRepository (DynamoDBContext dynamoDbContext, DynamoDBConfig dynamoDbConfig)
    {
        _dynamoDbContext = dynamoDbContext;
        _dbOperationConfig = new DynamoDBOperationConfig
        {
            TableNamePrefix = dynamoDbConfig.TablePrefix
        };
    }

    public async Task<Statistics> Get ()
    {
        var statistics = await _dynamoDbContext.LoadAsync<Statistics> (Statistics.DefaultId, _dbOperationConfig) ??
                         new Statistics ();

        return statistics;
    }

    public Task Update (Statistics statistics)
    {
        statistics.Id = Statistics.DefaultId;
        return _dynamoDbContext.SaveAsync (statistics, _dbOperationConfig);
    }
}