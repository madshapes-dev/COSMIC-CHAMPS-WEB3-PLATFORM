namespace ClientFilterInterceptor;

public class ClientFilterConfig
{
    public bool Maintenance { set; get; }
    public BuildVersions BuildVersions { set; get; }
    public string XUrl { set; get; }
    public string TelegramUrl { set; get; }
    public string DiscordUrl { set; get; }
}