using System;
using CosmicChamps.Data;
using CosmicChamps.Data.Sources.Tokens;

namespace CosmicChamps.Bootstrap.Simulator
{
    public class SimulatorTokensDataSource : ITokensDataSource
    {
        private readonly string _token;

        public SimulatorTokensDataSource (string token)
        {
            _token = token;
        }

        public Tokens Get () => new()
        {
            IdToken = _token,
            AccessToken = _token,
            RefreshToken = null,
            ExpirationDate = DateTimeOffset.Now.AddDays (365).ToUnixTimeSeconds ()
        };

        public void Set (Tokens tokens)
        {
        }

        public void Clear ()
        {
        }
    }
}