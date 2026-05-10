namespace CosmicChamps.Api.Model;

public interface ITournamentRepository
{
    Task<Tournament?> GetAsync (string id);
    Task UpdateAsync (Tournament tournament);
}