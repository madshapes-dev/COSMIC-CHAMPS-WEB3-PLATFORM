namespace CosmicChamps.Data.Sources.Tokens
{
    public interface ITokensDataSource
    {
        Data.Tokens Get ();
        void Set (Data.Tokens tokens);
        void Clear ();
    }
}