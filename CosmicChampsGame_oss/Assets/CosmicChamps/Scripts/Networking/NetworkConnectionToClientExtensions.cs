using Mirror;

namespace CosmicChamps.Networking
{
    public static class NetworkConnectionToClientExtensions
    {
        public static AuthData GetAuthData (this NetworkConnectionToClient conn) => conn.authenticationData as AuthData;
    }
}