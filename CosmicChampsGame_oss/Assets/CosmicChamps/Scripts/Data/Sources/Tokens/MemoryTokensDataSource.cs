namespace CosmicChamps.Data.Sources.Tokens
{
    public class MemoryTokensDataSource : ITokensDataSource
    {
        private Data.Tokens tokens;

        public Data.Tokens Get () => tokens;

        public void Set (Data.Tokens tokens) => this.tokens = tokens;

        public void Clear () => tokens = null;
    }
}