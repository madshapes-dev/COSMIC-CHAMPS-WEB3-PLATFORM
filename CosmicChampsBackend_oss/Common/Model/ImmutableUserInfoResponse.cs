namespace CosmicChamps.Common.Model;

public class ImmutableUserInfoResponse
{
    public string sub { set; get; }
    public string email { set; get; }
    public string passport_address { set; get; }
    public string[] linked_addresses { set; get; }
}