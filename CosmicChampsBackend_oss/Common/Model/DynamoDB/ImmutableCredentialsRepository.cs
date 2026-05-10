using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using CosmicChamps.Common.Configs;

namespace CosmicChamps.Common.Model.DynamoDB;

public class ImmutableCredentialsRepository : IImmutableCredentialsRepository
{
    private readonly DynamoDBContext _dynamoDbContext;
    private readonly DynamoDBOperationConfig _dbOperationConfig;

    public ImmutableCredentialsRepository (
        DynamoDBContext dynamoDbContext,
        DynamoDBConfig dynamoDbConfig)
    {
        _dynamoDbContext = dynamoDbContext;
        _dbOperationConfig = new DynamoDBOperationConfig
        {
            TableNamePrefix = dynamoDbConfig.TablePrefix
        };
    }

    public async Task CreateAsync (ImmutableCredentials immutableCredentials)
    {
        await _dynamoDbContext.SaveAsync (immutableCredentials, _dbOperationConfig);
    }

    public async Task<ImmutableCredentials?> GetAsync (string immutableId)
    {
        return await _dynamoDbContext.LoadAsync<ImmutableCredentials> (immutableId, _dbOperationConfig);
    }

    public async Task<ImmutableCredentials?> GetByEmailAsync (string email)
    {
        var scanConditions = new List<ScanCondition>
            { new(nameof (ImmutableCredentials.Email), ScanOperator.Equal, email) };

        var response = _dynamoDbContext.ScanAsync<ImmutableCredentials> (scanConditions, _dbOperationConfig);
        var credentials = await response.GetRemainingAsync ();

        return credentials.FirstOrDefault ();
    }

    public async Task<ImmutableCredentials?> GetByPlayerIdAsync (string playerId)
    {
        var scanConditions = new List<ScanCondition>
            { new(nameof (ImmutableCredentials.PlayerId), ScanOperator.Equal, playerId) };

        var response = _dynamoDbContext.ScanAsync<ImmutableCredentials> (scanConditions, _dbOperationConfig);
        var credentials = await response.GetRemainingAsync ();

        return credentials.FirstOrDefault ();
    }
}