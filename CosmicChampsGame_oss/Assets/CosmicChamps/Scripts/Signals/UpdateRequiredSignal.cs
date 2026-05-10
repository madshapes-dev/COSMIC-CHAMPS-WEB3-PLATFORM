namespace CosmicChamps.Signals
{
    public class UpdateRequiredSignal
    {
        public readonly string Message;
        public readonly string Link;
        public readonly bool Soft;

        public UpdateRequiredSignal (string message, string link, bool soft)
        {
            Message = message;
            Link = link;
            Soft = soft;
        }
    }
}