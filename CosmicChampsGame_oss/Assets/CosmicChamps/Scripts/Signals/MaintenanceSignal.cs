namespace CosmicChamps.Signals
{
    public class MaintenanceSignal
    {
        public readonly string XUrl;
        public readonly string TelegramUrl;
        public readonly string DiscordUrl;

        public MaintenanceSignal (string xUrl, string telegramUrl, string discordUrl)
        {
            XUrl = xUrl;
            TelegramUrl = telegramUrl;
            DiscordUrl = discordUrl;
        }
    }
}