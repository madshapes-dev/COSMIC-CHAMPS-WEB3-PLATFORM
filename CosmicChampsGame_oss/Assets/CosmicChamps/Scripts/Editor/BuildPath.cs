using System;
using UnityEditor;
using UnityEngine;

namespace CosmicChamps.Editor
{
    public interface IBuildPath
    {
        string GetBuildPath ();
    }

    public static class BuildPath
    {
        public static readonly ClientBuildPath Client = new();
        public static readonly ServerBuildPath Server = new();
        public static readonly SimulatorBuildPath Simulator = new();

        public class ClientBuildPath : IBuildPath
        {
            public string GetBuildPath (
                BuildTarget buildTarget,
                bool isDevelopmentBuild,
                bool androidAppBundle,
                AppIdentifier appIdentifier)
            {
                switch (buildTarget)
                {
                    case BuildTarget.Android:
                        return
                            $"Builds/Client/{buildTarget}/{Application.identifier}-{(isDevelopmentBuild ? "debug" : "release")}-{PlayerSettings.bundleVersion}-b{PlayerSettings.Android.bundleVersionCode}.{(androidAppBundle ? "aab" : "apk")}";
                    case BuildTarget.iOS:
                        return
                            $"Builds/Client/{buildTarget}/{Application.productName}-{(isDevelopmentBuild ? "debug" : "release")}";
                    case BuildTarget.StandaloneOSX:
                        return
                            $"Builds/Client/{buildTarget}/{(isDevelopmentBuild ? "Debug" : "Release")}/{appIdentifier.ProductName.Replace (" ", "")}.app";
                    case BuildTarget.StandaloneWindows64:
                        return
                            $"Builds/Client/{buildTarget}/{(isDevelopmentBuild ? "Debug" : "Release")}/{appIdentifier.ProductName.Replace (" ", "")}.exe";
                    case BuildTarget.WebGL:
                        return
                            $"Builds/Client/{buildTarget}/{(isDevelopmentBuild ? "Debug" : "Release")}";
                    default:
                        throw new NotImplementedException ();
                }
            }

            public string GetBuildPath ()
            {
                var buildTarget = EditorUserBuildSettings.activeBuildTarget;
                var isDevelopmentBuild = EditorUserBuildSettings.development;
                var androidAppBundle = EditorUserBuildSettings.buildAppBundle;

                switch (buildTarget)
                {
                    case BuildTarget.Android:
                        return
                            $"Builds/Client/{buildTarget}/{Application.identifier}-{(isDevelopmentBuild ? "debug" : "release")}-{PlayerSettings.bundleVersion}-b{PlayerSettings.Android.bundleVersionCode}.{(androidAppBundle ? "aab" : "apk")}";
                    case BuildTarget.iOS:
                        return
                            $"Builds/Client/{buildTarget}/{Application.productName}-{(isDevelopmentBuild ? "debug" : "release")}";
                    case BuildTarget.StandaloneOSX:
                        return
                            $"Builds/Client/{buildTarget}/{(isDevelopmentBuild ? "Debug" : "Release")}/{PlayerSettings.productName.Replace (" ", "")}.app";
                    case BuildTarget.StandaloneWindows64:
                        return
                            $"Builds/Client/{buildTarget}/{(isDevelopmentBuild ? "Debug" : "Release")}/{PlayerSettings.productName.Replace (" ", "")}.exe";
                    case BuildTarget.WebGL:
                        return
                            $"Builds/Client/{buildTarget}/{(isDevelopmentBuild ? "Debug" : "Release")}";
                    default:
                        throw new NotImplementedException ();
                }
            }
        }
    }

    public class SimulatorBuildPath : IBuildPath
    {
        public string GetBuildPath ()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var isDevelopmentBuild = EditorUserBuildSettings.development;

            switch (buildTarget)
            {
                case BuildTarget.StandaloneOSX:
                    return
                        $"Builds/Simulator/{buildTarget}/{(isDevelopmentBuild ? "Debug" : "Release")}";
                default:
                    throw new NotImplementedException ();
            }
        }
    }

    public class ServerBuildPath : IBuildPath
    {
        public string GetBuildPath ()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var isDevelopmentBuild = EditorUserBuildSettings.development;

            switch (buildTarget)
            {
                case BuildTarget.StandaloneLinux64:
                    return
                        $"Builds/Server/{buildTarget}/{(isDevelopmentBuild ? "Debug" : "Release")}/{PlayerSettings.productName}";
                case BuildTarget.StandaloneOSX:
                    return
                        $"Builds/Server/{buildTarget}/{(isDevelopmentBuild ? "Debug" : "Release")}";
                case BuildTarget.StandaloneWindows64:
                    return
                        $"Builds/Server/{buildTarget}/{(isDevelopmentBuild ? "Debug" : "Release")}/{PlayerSettings.productName}.exe";
                default:
                    throw new NotImplementedException ();
            }
        }
    }
}