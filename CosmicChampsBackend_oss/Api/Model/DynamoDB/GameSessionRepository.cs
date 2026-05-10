using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using CosmicChamps.Api.Configs;
using CosmicChamps.Common.Configs;

namespace CosmicChamps.Api.Model.DynamoDB;

public class GameSessionRepository : IGameSessionRepository
{
    private readonly DynamoDBContext _dynamoDbContext;
    private readonly DynamoDBOperationConfig _dbOperationConfig;

    public GameSessionRepository (DynamoDBContext dynamoDbContext, DynamoDBConfig dynamoDbConfig)
    {
        _dynamoDbContext = dynamoDbContext;
        _dbOperationConfig = new DynamoDBOperationConfig
        {
            TableNamePrefix = dynamoDbConfig.TablePrefix
        };
    }

    public async Task CreateAsync (GameSession gameSession)
    {
        await _dynamoDbContext.SaveAsync (gameSession, _dbOperationConfig);
    }

    public async Task<GameSession> GetAsync (string id)
    {
        return await _dynamoDbContext.LoadAsync<GameSession> (id, _dbOperationConfig);
    }

    public async Task UpdateAsync (GameSession gameSession)
    {
        await _dynamoDbContext.SaveAsync (gameSession, _dbOperationConfig);
    }

    public async Task DeleteAsync (string id)
    {
        await _dynamoDbContext.DeleteAsync<GameSession> (id, _dbOperationConfig);
    }

    public async Task<GameSession?> GetByPlayerId (string playerId, string tournamentId)
    {
        var scanConditions = new List<ScanCondition>
            { new(nameof (GameSession.PlayerIds), ScanOperator.Contains, playerId) };

        var response = _dynamoDbContext.ScanAsync<GameSession> (scanConditions, _dbOperationConfig);
        var gameSessions = await response.GetRemainingAsync ();

        var gameSession = string.IsNullOrEmpty (tournamentId)
            ? gameSessions.FirstOrDefault (x => string.IsNullOrEmpty (x.TournamentId))
            : gameSessions.FirstOrDefault (x => x.TournamentId == tournamentId);

        return gameSession;
    }
}