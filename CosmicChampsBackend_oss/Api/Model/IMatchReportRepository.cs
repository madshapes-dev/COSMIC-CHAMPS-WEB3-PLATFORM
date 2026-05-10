namespace CosmicChamps.Api.Model;

public interface IMatchReportRepository
{
    Task CreateAsync (MatchReport player);
}