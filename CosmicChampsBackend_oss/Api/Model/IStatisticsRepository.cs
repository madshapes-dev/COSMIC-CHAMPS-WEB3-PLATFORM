namespace CosmicChamps.Api.Model;

public interface IStatisticsRepository
{
    Task<Statistics> Get ();
    Task Update (Statistics statistics);
}