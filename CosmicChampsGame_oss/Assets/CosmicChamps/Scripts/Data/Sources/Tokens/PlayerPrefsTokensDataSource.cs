using CosmicChamps.Services;

namespace CosmicChamps.Data.Sources.Tokens
{
    public class PlayerPrefsTokensDataSource : ITokensDataSource
    {
        private readonly PlayerPrefsService _playerPrefsService;

        public PlayerPrefsTokensDataSource (PlayerPrefsService playerPrefsService)
        {
            _playerPrefsService = playerPrefsService;
        }

        public Data.Tokens Get () => _playerPrefsService.Tokens.Value;

        public void Set (Data.Tokens tokens) => _playerPrefsService.Tokens.Value = tokens;

        public void Clear () => _playerPrefsService.Tokens.Clear ();
    }
}