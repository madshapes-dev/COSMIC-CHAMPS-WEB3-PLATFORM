using Amazon.DynamoDBv2.DataModel;
using CosmicChamps.Common.Configs;

namespace CosmicChamps.Common.Model.DynamoDB;

public class GuestCredentialsRepository : IGuestCredentialsRepository
{
    private readonly DynamoDBContext _dynamoDbContext;
    private readonly DynamoDBOperationConfig _dbOperationConfig;

    public GuestCredentialsRepository (
        DynamoDBContext dynamoDbContext,
        DynamoDBConfig dynamoDbConfig)
    {
        _dynamoDbContext = dynamoDbContext;
        _dbOperationConfig = new DynamoDBOperationConfig
        {
            TableNamePrefix = dynamoDbConfig.TablePrefix
        };
    }

    public async Task CreateAsync (GuestCredentials guestCredentials)
    {
        await _dynamoDbContext.SaveAsync (guestCredentials, _dbOperationConfig);
    }

    public async Task<GuestCredentials?> GetAsync (string deviceId)
    {
        return await _dynamoDbContext.LoadAsync<GuestCredentials> (deviceId, _dbOperationConfig);
    }

    /*public async Task<GuestCredentials?> GetByPlayerIdAsync (string playerId)
    {
        var scanConditions = new List<ScanCondition>
            { new(nameof (GuestCredentials.PlayerId), ScanOperator.Equal, playerId) };

        var response = _dynamoDbContext.ScanAsync<GuestCredentials> (scanConditions, _dbOperationConfig);
        var guestCredentials = await response.GetRemainingAsync ();

        return guestCredentials.FirstOrDefault ();
    }*/

    public async Task DeleteAsync (string deviceId)
    {
        await _dynamoDbContext.DeleteAsync<GuestCredentials> (deviceId, _dbOperationConfig);
    }
}