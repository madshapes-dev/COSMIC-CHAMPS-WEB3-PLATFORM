namespace CosmicChamps.Common.Model;

public interface IGuestCredentialsRepository
{
    Task CreateAsync (GuestCredentials guestCredentials);
    Task<GuestCredentials?> GetAsync (string deviceId);
    Task DeleteAsync (string deviceId);
}