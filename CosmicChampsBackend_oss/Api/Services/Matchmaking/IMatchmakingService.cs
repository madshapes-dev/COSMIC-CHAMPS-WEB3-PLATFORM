namespace CosmicChamps.Api.Services.Matchmaking;

public interface IMatchmakingService
{
    Task<string> StartMatchmaking (string playerId, string walletId, string tournamentId);
    Task<PlayerGameSession> GetPlayerGameSession (string playerId, string ticketId);
    Task CancelTicket (string ticketId);
}