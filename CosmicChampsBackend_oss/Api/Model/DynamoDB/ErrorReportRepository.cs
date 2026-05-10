using Amazon.DynamoDBv2.DataModel;
using CosmicChamps.Common.Configs;

namespace CosmicChamps.Api.Model.DynamoDB;

public class ErrorReportRepository : IErrorReportRepository
{
    private readonly DynamoDBContext _dynamoDbContext;
    private readonly DynamoDBConfig _dynamoDbConfig;
    private readonly DynamoDBOperationConfig _dbOperationConfig;

    public ErrorReportRepository (DynamoDBContext dynamoDbContext, DynamoDBConfig dynamoDbConfig)
    {
        _dynamoDbContext = dynamoDbContext;
        _dynamoDbConfig = dynamoDbConfig;
        _dbOperationConfig = new DynamoDBOperationConfig
        {
            TableNamePrefix = _dynamoDbConfig.TablePrefix
        };
    }

    public async Task CreateAsync (ErrorReport errorReport)
    {
        await _dynamoDbContext.SaveAsync (errorReport, _dbOperationConfig);
    }
}