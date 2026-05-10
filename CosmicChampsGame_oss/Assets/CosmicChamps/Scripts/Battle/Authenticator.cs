using System;
using CosmicChamps.Networking;
using Mirror;

namespace CosmicChamps.Battle
{
    public class Authenticator : NetworkAuthenticator
    {
        private AuthData _authData;

        public void SetAuthData (AuthData authData) => _authData = authData;

        #region Messages

        public struct AuthRequestMessage : NetworkMessage
        {
            public AuthData AuthData;
        }

        public struct AuthResponseMessage : NetworkMessage
        {
        }

        #endregion

        #region Server

        private void OnAuthRequestMessage (NetworkConnectionToClient conn, AuthRequestMessage msg)
        {
            conn.authenticationData = msg.AuthData;
            conn.Send (new AuthResponseMessage ());

            ServerAccept (conn);
        }

        public override void OnStartServer ()
        {
            NetworkServer.RegisterHandler<AuthRequestMessage> (OnAuthRequestMessage, false);
        }

        public override void OnStopServer ()
        {
            NetworkServer.UnregisterHandler<AuthRequestMessage> ();
        }

        public override void OnServerAuthenticate (NetworkConnectionToClient conn)
        {
            // do nothing, wait for client to send his id
        }

        #endregion

        #region Client

        public override void OnStartClient ()
        {
            NetworkClient.RegisterHandler ((Action<AuthResponseMessage>)OnAuthResponseMessage, false);
        }

        public override void OnStopClient ()
        {
            NetworkClient.UnregisterHandler<AuthResponseMessage> ();
        }

        public override void OnClientAuthenticate ()
        {
            NetworkClient.connection.Send (new AuthRequestMessage { AuthData = _authData });
        }

        public void OnAuthResponseMessage (AuthResponseMessage msg)
        {
            ClientAccept ();
        }

        #endregion
    }
}