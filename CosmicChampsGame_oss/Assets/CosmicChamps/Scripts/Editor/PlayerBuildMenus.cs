using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;


namespace CosmicChamps.Editor
{
    public static class PlayerBuildMenus
    {
        private const string VersionTemplate = @"#!/bin/bash

ENV=""{ENV}""
VERSION=""{VERSION}""
BUILD=""b{BUILD}""";

        private const string CleanBuildMenuItem = "CosmicChamps/Build/Clean Build";

        private static void MakeBuildUsingProfile (string profile, IBuildPath buildPath)
        {
            VersionHandler.UpdateBuildInfoDate ();

            Debug.Log ($"[Player Build] Switch Build Profile to ${profile}...");
            var buildProfile = BuildProfiles.SwitchBuildProfile (profile);

            Debug.Log ($"[Player Build] Setting version {BuildInfo.Version} (b{BuildInfo.BuildVersion})...");
            PlayerSettings.bundleVersion = BuildInfo.Version;
            PlayerSettings.Android.bundleVersionCode = BuildInfo.BuildVersion;
            PlayerSettings.iOS.buildNumber = BuildInfo.BuildVersion.ToString ();


            var locationPath = buildPath.GetBuildPath ();
            var options = EditorPrefs.GetBool (CleanBuildMenuItem) ? BuildOptions.CleanBuildCache : BuildOptions.None;

            Debug.Log (
                $"[Player Build] Building {(options == BuildOptions.CleanBuildCache ? "clean" : "incremental")} player...");

            var report = BuildPipeline.BuildPlayer (
                new BuildPlayerWithProfileOptions
                {
                    buildProfile = buildProfile,
                    locationPathName = locationPath,
                    options = options
                });

            var summary = report.summary;
            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log ("[Player Build] Build succeeded: " + summary.totalSize + " bytes");

                    var guids = AssetDatabase.FindAssets ("t:prefab ProjectContext");
                    if (guids.Length == 0)
                        throw new InvalidOperationException ("Unable to find ProjectContext");

                    var directory = File.Exists (locationPath) ? Path.GetDirectoryName (locationPath) : locationPath;
                    var projectRootInstaller = AppProfileMenus.GetRootInstaller ();

                    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
                        File.WriteAllText (
                            Path.Combine (directory, "version"),
                            VersionTemplate
                                .Replace ("{ENV}", projectRootInstaller.AppProfile.name.Replace ("AppProfile", string.Empty))
                                .Replace ("{VERSION}", BuildInfo.Version)
                                .Replace ("{BUILD}", BuildInfo.BuildVersion.ToString ()));

                    break;
                case BuildResult.Failed:
                    Debug.LogError ("[Player Build] Build failed");
                    break;
                case BuildResult.Unknown:
                    Debug.LogWarning ("[Player Build] Build unknown result");
                    break;
                case BuildResult.Cancelled:
                    Debug.Log ("[Player Build] Build canceled");
                    break;
                default:
                    throw new ArgumentOutOfRangeException ();
            }
        }

        [MenuItem (CleanBuildMenuItem, priority = 5)]
        public static void CleanBuild ()
        {
            var cleanBuild = EditorPrefs.GetBool (CleanBuildMenuItem);
            EditorPrefs.SetBool (CleanBuildMenuItem, !cleanBuild);
        }

        [MenuItem (CleanBuildMenuItem, true)]
        private static bool CleanBuildValidate ()
        {
            Menu.SetChecked (CleanBuildMenuItem, EditorPrefs.GetBool (CleanBuildMenuItem));
            return true;
        }

        [MenuItem ("CosmicChamps/Build/Simulator/macOS/Release", priority = 6)]
        public static void BuildSimulatorMacOSRelease ()
        {
            MakeBuildUsingProfile (BuildProfiles.macOS.Release.Simulator, BuildPath.Simulator);
        }

        [MenuItem ("CosmicChamps/Build/Client/macOS/Release/Primary", priority = 6)]
        public static void BuildPrimaryMacOSRelease ()
        {
            MakeBuildUsingProfile (BuildProfiles.macOS.Release.Primary, BuildPath.Client);
        }

        [MenuItem ("CosmicChamps/Build/Client/macOS/Release/Secondary", priority = 6)]
        public static void BuildSecondaryMacOSRelease ()
        {
            MakeBuildUsingProfile (BuildProfiles.macOS.Release.Secondary, BuildPath.Client);
        }

        [MenuItem ("CosmicChamps/Build/Client/macOS/Release/Both", priority = 6)]
        public static void BuildBothMacOSRelease ()
        {
            MakeBuildUsingProfile (BuildProfiles.macOS.Release.Primary, BuildPath.Client);
            MakeBuildUsingProfile (BuildProfiles.macOS.Release.Secondary, BuildPath.Client);
        }

        [MenuItem ("CosmicChamps/Build/Client/macOS/Debug/Primary", priority = 6)]
        public static void BuildPrimaryMacOSDebug ()
        {
            MakeBuildUsingProfile (BuildProfiles.macOS.Debug.Primary, BuildPath.Client);
        }

        [MenuItem ("CosmicChamps/Build/Client/macOS/Debug/Secondary", priority = 6)]
        public static void BuildSecondaryMacOSDebug ()
        {
            MakeBuildUsingProfile (BuildProfiles.macOS.Debug.Secondary, BuildPath.Client);
        }

        [MenuItem ("CosmicChamps/Build/Client/macOS/Debug/Both", priority = 6)]
        public static void BuildBothMacOSDebug ()
        {
            MakeBuildUsingProfile (BuildProfiles.macOS.Debug.Primary, BuildPath.Client);
            MakeBuildUsingProfile (BuildProfiles.macOS.Debug.Secondary, BuildPath.Client);
        }

        [MenuItem ("CosmicChamps/Build/Client/Android/Release", priority = 6)]
        public static void BuildAndroidRelease ()
        {
            MakeBuildUsingProfile (BuildProfiles.Android.Release, BuildPath.Client);
        }

        [MenuItem ("CosmicChamps/Build/Client/Android/Debug", priority = 6)]
        public static void BuildAndroidDebug ()
        {
            MakeBuildUsingProfile (BuildProfiles.Android.Debug, BuildPath.Client);
        }

        [MenuItem ("CosmicChamps/Build/Client/iOS/Release", priority = 6)]
        public static void BuildIOSRelease ()
        {
            MakeBuildUsingProfile (BuildProfiles.iOS.Release, BuildPath.Client);
        }

        [MenuItem ("CosmicChamps/Build/Client/iOS/Debug", priority = 6)]
        public static void BuildIOSDebug ()
        {
            MakeBuildUsingProfile (BuildProfiles.iOS.Debug, BuildPath.Client);
        }

        [MenuItem ("CosmicChamps/Build/Client/WebGL/Release", priority = 6)]
        public static void BuildWebGLRelease ()
        {
            MakeBuildUsingProfile (BuildProfiles.Web.Release, BuildPath.Client);
        }

        [MenuItem ("CosmicChamps/Build/Client/WebGL/Debug", priority = 6)]
        public static void BuildWebGLDebug ()
        {
            MakeBuildUsingProfile (BuildProfiles.Web.Debug, BuildPath.Client);
        }

        [MenuItem ("CosmicChamps/Build/Server/Linux/Release", priority = 6)]
        public static void BuildLinuxRelease ()
        {
            MakeBuildUsingProfile (BuildProfiles.Linux.Release.Server, BuildPath.Server);
        }

        [MenuItem ("CosmicChamps/Build/Server/Linux/Debug", priority = 6)]
        public static void BuildLinuxDebug ()
        {
            MakeBuildUsingProfile (BuildProfiles.Linux.Debug.Server, BuildPath.Server);
        }

        [MenuItem ("CosmicChamps/Build/Server/macOS/Release", priority = 6)]
        public static void BuildMacOSRelease ()
        {
            MakeBuildUsingProfile (BuildProfiles.macOS.Release.Server, BuildPath.Server);
        }

        [MenuItem ("CosmicChamps/Build/Server/macOS/Debug", priority = 6)]
        public static void BuildMacOSDebug ()
        {
            MakeBuildUsingProfile (BuildProfiles.macOS.Debug.Server, BuildPath.Server);
        }
    }
}