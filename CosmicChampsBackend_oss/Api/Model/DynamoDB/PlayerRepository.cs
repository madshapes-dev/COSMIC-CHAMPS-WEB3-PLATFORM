using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using CosmicChamps.Common.Configs;

namespace CosmicChamps.Api.Model.DynamoDB;

public class PlayerRepository : IPlayerRepository
{
    private readonly DynamoDBContext _dynamoDbContext;
    private readonly AmazonDynamoDBClient _dbClient;
    private readonly DynamoDBOperationConfig _dbOperationConfig;

    public PlayerRepository (DynamoDBContext dynamoDbContext, DynamoDBConfig dynamoDbConfig, AmazonDynamoDBClient dbClient)
    {
        _dynamoDbContext = dynamoDbContext;
        _dbClient = dbClient;
        _dbOperationConfig = new DynamoDBOperationConfig
        {
            TableNamePrefix = dynamoDbConfig.TablePrefix
        };
    }

    public async Task CreateAsync (Player player)
    {
        await _dynamoDbContext.SaveAsync (player, _dbOperationConfig);
    }

    public async Task<Player?> GetAsync (string id)
    {
        return await _dynamoDbContext.LoadAsync<Player> (id, _dbOperationConfig);
    }

    public async Task<ICollection<Player>> GetAsync (ICollection<string> ids)
    {
        var players = await _dynamoDbContext
            .ScanAsync<Player> (
                new[] { new ScanCondition (nameof (Player.Id), ScanOperator.In, ids.Select (x => x as object).ToArray ()) },
                _dbOperationConfig)
            .GetRemainingAsync ();

        return players;
    }

    public async Task UpdateAsync (Player player)
    {
        await _dynamoDbContext.SaveAsync (player, _dbOperationConfig);
    }

    public async Task DeleteAsync (string id)
    {
        await _dynamoDbContext.DeleteAsync<Player> (id, _dbOperationConfig);
    }

    public async Task<bool> IsNicknameUnique (string nickname)
    {
        var players = await _dynamoDbContext
            .ScanAsync<Player> (
                new[] { new ScanCondition (nameof (Player.Nickname), ScanOperator.Equal, nickname) },
                _dbOperationConfig)
            .GetNextSetAsync ();

        return players.Count == 0;
    }

    public async Task<List<string>> GetPrefixedNicknames (string prefix)
    {
        var nicknames = (await _dbClient
                .ScanAsync (
                    $"{_dbOperationConfig.TableNamePrefix}{nameof (Player)}",
                    new List<string> { nameof (Player.Nickname) },
                    new Dictionary<string, Condition>
                    {
                        {
                            nameof (Player.Nickname), new Condition
                            {
                                AttributeValueList = new List<AttributeValue> { new(prefix) },
                                ComparisonOperator = ComparisonOperator.BEGINS_WITH
                            }
                        }
                    }))
            .Items
            .Select (x => x.First ().Value.S)
            .ToList ();

        return nicknames;
    }
}