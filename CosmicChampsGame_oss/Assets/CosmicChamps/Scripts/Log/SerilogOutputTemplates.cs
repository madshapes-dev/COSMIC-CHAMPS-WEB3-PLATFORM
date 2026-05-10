namespace CosmicChamps.Log
{
    public static class SerilogOutputTemplates
    {
        public const string Editor =
            "<color={UnityEditorLogLevelColor}><b>[Frame:{UnityFrameCount} {Level:u3}/{UnityCustomTag}</b><b>]:</b></color> {Message:lj}{NewLine}{Exception}";

        public const string Build = "[Frame:{UnityFrameCount} {Level:u3}/{UnityCustomTag}]: {Message:lj}{NewLine}{Exception}";
        public const string CloudWatch = "[{Level:u3}/{UnityCustomTag}]: {Message:lj}{NewLine}{Exception}";
    }
}