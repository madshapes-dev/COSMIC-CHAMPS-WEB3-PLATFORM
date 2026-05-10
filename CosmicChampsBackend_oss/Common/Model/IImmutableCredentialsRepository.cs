namespace CosmicChamps.Common.Model;

public interface IImmutableCredentialsRepository
{
    Task CreateAsync (ImmutableCredentials immutableCredentials);
    Task<ImmutableCredentials?> GetAsync (string immutableId);
    Task<ImmutableCredentials?> GetByEmailAsync (string email);
    Task<ImmutableCredentials?> GetByPlayerIdAsync (string playerId);
}