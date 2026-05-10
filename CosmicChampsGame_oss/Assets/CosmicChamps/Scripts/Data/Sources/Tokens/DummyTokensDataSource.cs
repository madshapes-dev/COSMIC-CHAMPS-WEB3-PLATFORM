namespace CosmicChamps.Data.Sources.Tokens
{
    public class DummyTokensDataSource : ITokensDataSource
    {
        public Data.Tokens Get () => new();

        public void Set (Data.Tokens tokens)
        {
        }

        public void Clear ()
        {
        }
    }
}