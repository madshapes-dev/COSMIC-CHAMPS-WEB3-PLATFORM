namespace CosmicChamps.Common.Configs;

public class AWSConfig
{
    public string Key { set; get; }
    public string Secret { set; get; }
    public AWSCognitoConfig Cognito { set; get; }
}
