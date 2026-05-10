using UnityEngine;

namespace CosmicChamps
{
    [CreateAssetMenu (fileName = nameof (BuildInfo), menuName = "Cosmic Champs/Build Info")]
    public class BuildInfo : ScriptableObject
    {
        private static BuildInfo _instance;

        public static readonly string VersionProperty = nameof (_version);
        public static readonly string BuildVersionProperty = nameof (_buildVersion);
        public static readonly string DateProperty = nameof (_date);

        [SerializeField]
        private string _version;

        [SerializeField]
        private int _buildVersion;

        [SerializeField]
        private string _date;

        private static BuildInfo Instance => _instance ??= Resources.Load<BuildInfo> (nameof (BuildInfo));

        /*public const string Version = "0.17.0";
        public const int BuildVersion = 170000;
        public const string Date = "26.Aug.2024";
        public static readonly string AppVersionString = $"cbeta-{Date}-{Version}(b{BuildVersion})";*/

        public static string Version => Instance._version;
        public static int BuildVersion => Instance._buildVersion;
        public static string Date => Instance._date;
        public static string AppVersionString => $"cbeta-{Date}-{Version}(b{BuildVersion})";
    }
}