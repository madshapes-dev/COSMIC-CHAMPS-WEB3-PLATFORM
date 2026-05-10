using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using CosmicChamps.Api.Configs;
using CosmicChamps.Common.Configs;
using FluentDateTimeOffset;

namespace CosmicChamps.Api.Model.DynamoDB;

public class NewsRepository : INewsRepository
{
    private readonly DynamoDBContext _dynamoDbContext;
    private readonly DynamoDBOperationConfig _dbOperationConfig;

    public NewsRepository (DynamoDBContext dynamoDbContext, DynamoDBConfig dynamoDbConfig)
    {
        _dynamoDbContext = dynamoDbContext;
        _dbOperationConfig = new DynamoDBOperationConfig
        {
            TableNamePrefix = dynamoDbConfig.TablePrefix
        };
    }

    public async Task<News?> GetLatest ()
    {
        /*var news = await _dynamoDbContext
            .FromQueryAsync<News> (
                new QueryOperationConfig
                {
                    Limit = 1,
                    Select = SelectValues.AllAttributes,
                    ConsistentRead = true,
                    Filter = new QueryFilter (nameof (News.Id), QueryOperator.Equal, "Id"),
                    BackwardSearch = true
                },
                _dbOperationConfig)
            .GetNextSetAsync ();*/
        var now = DateTimeOffset.UtcNow;
        var tomorrow = now.NextDay ().Midnight ().ToString ("yyyy-MM-ddTHH:mm");
        var threeDaysEarlier = now.PreviousDay ().PreviousDay ().Midnight ().ToString ("yyyy-MM-ddTHH:mm");
        var filter = new ScanFilter ();
        filter.AddCondition (
            nameof (News.Id),
            ScanOperator.Between,
            threeDaysEarlier,
            tomorrow);
        var news = await _dynamoDbContext
            .FromScanAsync<News> (
                new ScanOperationConfig
                {
                    ConsistentRead = true,
                    Filter = filter
                },
                _dbOperationConfig)
            .GetRemainingAsync ();

        return news.Count == 0
            ? null
            : news.OrderByDescending (x => x.Id).First ();
    }
}