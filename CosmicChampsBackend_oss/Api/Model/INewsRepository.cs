namespace CosmicChamps.Api.Model;

public interface INewsRepository
{
    Task<News?> GetLatest ();
}