namespace CosmicChamps.Api.Configs;

public class CustomJWTConfig
{
    public string Secret { set; get; }
    public string Issuer { set; get; }
    public string Audience { set; get; }
}