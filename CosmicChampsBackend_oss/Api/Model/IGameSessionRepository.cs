namespace CosmicChamps.Api.Model;

public interface IGameSessionRepository
{
    Task CreateAsync (GameSession gameSession);
    Task<GameSession> GetAsync (string id);
    Task UpdateAsync (GameSession gameSession);
    Task DeleteAsync (string id);
    Task<GameSession?> GetByPlayerId (string playerId, string tournamentId);
}