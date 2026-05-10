namespace CosmicChamps
{
    public static class CommandLineOptions
    {
        public const string OptionPrefix = "-";
        
        public static readonly string ClearAddressables = $"{OptionPrefix}{nameof (ClearAddressables)}";
        public static readonly string ClearPrefs = $"{OptionPrefix}{nameof (ClearPrefs)}";
        public static readonly string Port = $"{OptionPrefix}{nameof (Port)}";
        public static readonly string LogFile = $"{OptionPrefix}{nameof (LogFile)}";
        public static readonly string Token = $"{OptionPrefix}{nameof (Token)}";
    }
}