using UnityEditor;
using UnityEditor.Build.Profile;

namespace CosmicChamps.Editor
{
    public static class BuildProfiles
    {
        private const string Path = "Assets/Settings/Build Profiles";

        public static class Android
        {
            public const string Debug = "Android/AndroidDebug";
            public const string Release = "Android/AndroidRelease";
        }

        public static class iOS
        {
            public const string Debug = "iOS/iOSDebug";
            public const string Release = "iOS/iOSRelease";
        }

        public static class Web
        {
            public const string Debug = "Web/WebDebug";
            public const string Release = "Web/WebRelease";
        }

        public static class macOS
        {
            public static class Debug
            {
                public const string Primary = "macOS/Primary/macOSPrimaryDebug";
                public const string Secondary = "macOS/Secondary/macOSSecondaryDebug";
                public const string Server = "macOS/Server/macOSServerDebug";
            }

            public static class Release
            {
                public const string Primary = "macOS/Primary/macOSPrimaryRelease";
                public const string Secondary = "macOS/Secondary/macOSSecondaryRelease";
                public const string Server = "macOS/Server/macOSServerRelease";
                public const string Simulator = "macOS/Simulator/macOSSimulatorRelease";
            }
        }

        public static class Linux
        {
            public static class Debug
            {
                public const string Server = "Linux/Server/LinuxServerDebug";
            }

            public static class Release
            {
                public const string Server = "Linux/Server/LinuxServerRelease";
            }
        }

        public static BuildProfile SwitchBuildProfile (string profile)
        {
            var buildProfile = AssetDatabase.LoadAssetAtPath<BuildProfile> ($"{Path}/{profile}.asset");
            var currentBuildProfile = BuildProfile.GetActiveBuildProfile ();
            
            if (currentBuildProfile == buildProfile)
                return buildProfile;

            BuildProfile.SetActiveBuildProfile (buildProfile);
            return buildProfile;
        }

        public static bool IsCurrentProfileServer ()
        {
            var buildProfile = BuildProfile.GetActiveBuildProfile ();
            var path = AssetDatabase.GetAssetPath (buildProfile);

            return path.Contains (Linux.Debug.Server) ||
                   path.Contains (Linux.Release.Server) ||
                   path.Contains (macOS.Debug.Server) ||
                   path.Contains (macOS.Release.Server);
        }
    }
}