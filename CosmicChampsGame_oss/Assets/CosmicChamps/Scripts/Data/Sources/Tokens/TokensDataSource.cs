namespace CosmicChamps.Data.Sources.Tokens
{
    public class TokensDataSource : ITokensDataSource
    {
        private readonly AppProfileTokensDataSource _appProfileTokensDataSource;
        private readonly MemoryTokensDataSource _memoryTokensDataSource;
        private readonly PlayerPrefsTokensDataSource _playerPrefsTokensDataSource;

        public TokensDataSource (
            AppProfileTokensDataSource appProfileTokensDataSource,
            MemoryTokensDataSource memoryTokensDataSource,
            PlayerPrefsTokensDataSource playerPrefsTokensDataSource)
        {
            _appProfileTokensDataSource = appProfileTokensDataSource;
            _memoryTokensDataSource = memoryTokensDataSource;
            _playerPrefsTokensDataSource = playerPrefsTokensDataSource;
        }

        public Data.Tokens Get ()
        {
            var tokens = _appProfileTokensDataSource.Get () ?? _memoryTokensDataSource.Get ();
            if (tokens != null)
                return tokens;

            tokens = _playerPrefsTokensDataSource.Get ();
            _memoryTokensDataSource.Set (tokens);

            return tokens;
        }

        public void Set (Data.Tokens tokens)
        {
            _memoryTokensDataSource.Set (tokens);
            _playerPrefsTokensDataSource.Set (tokens);
        }

        public void Clear ()
        {
            _memoryTokensDataSource.Clear ();
            _playerPrefsTokensDataSource.Clear ();
        }
    }
}