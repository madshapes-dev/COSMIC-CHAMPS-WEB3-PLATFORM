using UnityEngine;

namespace CosmicChamps.Settings
{
    public class GameLiftInstanceMetadata
    {
        public static string Path =>
            Application.platform switch
            {
                RuntimePlatform.LinuxServer => "/local/game/instance-metadata.json",
                RuntimePlatform.WindowsServer => "C:\\Game\\instance-metadata.json",
                _ => null
            };

        public string instanceId { set; get; }
    }
}