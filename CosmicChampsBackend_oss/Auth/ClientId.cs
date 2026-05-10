namespace CosmicChamps.Auth;

public class ClientId
{
    private readonly string _clientId;

    private ClientId (string clientId)
    {
        _clientId = clientId;
    }

    public static implicit operator string (ClientId clientId) => clientId._clientId;
    public static implicit operator ClientId (string clientId) => new(clientId);
}