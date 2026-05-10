using UnityEngine;

namespace CosmicChamps.Settings
{
    public class GameLiftMetadata
    {
        public static string Path =>
            Application.platform switch
            {
                RuntimePlatform.LinuxServer => "/local/gamemetadata/gamelift-metadata.json",
                RuntimePlatform.WindowsServer => "C:\\GameMetadata\\gamelift-metadata.json",
                _ => null
            };

        public string buildArn { set; get; }
        public string buildId { set; get; }
        public string fleetArn { set; get; }
        public string fleetId { set; get; }
        public string description { set; get; }
        public string name { set; get; }
        public string fleetType { set; get; }
        public string instanceType { set; get; }
        public string instanceRoleArn { set; get; }
    }
}