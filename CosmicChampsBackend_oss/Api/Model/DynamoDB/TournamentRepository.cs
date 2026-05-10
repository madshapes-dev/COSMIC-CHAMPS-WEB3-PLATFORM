using Amazon.DynamoDBv2.DataModel;
using CosmicChamps.Api.Configs;
using CosmicChamps.Common.Configs;

namespace CosmicChamps.Api.Model.DynamoDB;

public class TournamentRepository : ITournamentRepository
{
    private readonly DynamoDBContext _dynamoDbContext;
    private readonly DynamoDBOperationConfig _dbOperationConfig;

    public TournamentRepository (DynamoDBContext dynamoDbContext, DynamoDBConfig dynamoDbConfig)
    {
        _dynamoDbContext = dynamoDbContext;
        _dbOperationConfig = new DynamoDBOperationConfig
        {
            TableNamePrefix = dynamoDbConfig.TablePrefix
        };
    }

    public async Task<Tournament?> GetAsync (string id) =>
        await _dynamoDbContext.LoadAsync<Tournament> (id, _dbOperationConfig);

    public async Task UpdateAsync (Tournament tournament) =>
        await _dynamoDbContext.SaveAsync (tournament, _dbOperationConfig);
}