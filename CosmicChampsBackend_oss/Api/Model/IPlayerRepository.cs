namespace CosmicChamps.Api.Model;

public interface IPlayerRepository
{
    Task CreateAsync (Player player);
    Task<Player?> GetAsync (string id);
    Task<ICollection<Player>> GetAsync (ICollection<string> ids);
    Task UpdateAsync (Player player);
    Task DeleteAsync (string id);
    Task<bool> IsNicknameUnique (string nickname);
    Task<List<string>> GetPrefixedNicknames (string prefix);
}