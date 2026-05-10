using System;
using CosmicChamps.Settings;

namespace CosmicChamps.Data.Sources.Tokens
{
    public class AppProfileTokensDataSource : ITokensDataSource
    {
        private readonly AppProfile _appProfile;

        public AppProfileTokensDataSource (AppProfile appProfile)
        {
            _appProfile = appProfile;
        }

        public Data.Tokens Get ()
        {
            if (string.IsNullOrEmpty (_appProfile.CustomToken))
                return null;

            return new Data.Tokens
            {
                IdToken = _appProfile.CustomToken,
                AccessToken = _appProfile.CustomToken,
                RefreshToken = null,
                ExpirationDate = DateTimeOffset.Now.AddDays (365).ToUnixTimeSeconds ()
            };
        }

        public void Set (Data.Tokens tokens)
        {
        }

        public void Clear ()
        {
        }
    }
}