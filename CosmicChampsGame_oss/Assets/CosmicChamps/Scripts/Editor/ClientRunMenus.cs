using System.Diagnostics;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CosmicChamps.Editor
{
    public static class ClientRunMenus
    {
        private static async void RunInstances (
            BuildTarget buildTarget,
            bool debug,
            params AppIdentifier[] appIdentifiers)
        {
            for (var i = 0; i < appIdentifiers.Length; i++)
            {
                Debug.Log (
                    $"{Application.dataPath.Replace ("Assets", null)}{BuildPath.Client.GetBuildPath (buildTarget, debug, false, appIdentifiers[i])}");

                new Process
                    {
                        StartInfo =
                        {
                            FileName = "open",
                            Arguments =
                                $"-n -a {Application.dataPath.Replace ("Assets", null)}{BuildPath.Client.GetBuildPath (buildTarget, debug, false, appIdentifiers[i])}"
                        }
                    }
                    .Start ();

                await Task.Delay (2000);
            }

            new Process
            {
                StartInfo =
                {
                    FileName = "osascript",
                    Arguments = "-e 'tell application \"" +
                                PlayerSettings.productName +
                                "\"\r\n\tactivate\r\nend tell'"
                }
            }.Start ();
        }

        [MenuItem ("CosmicChamps/Run/macOS/Release/Primary", priority = 4)]
        public static void RunPrimaryOSXReleaseInstance ()
        {
            RunInstances (BuildTarget.StandaloneOSX, false, AppIdentifier.StandalonePrimary);
        }

        [MenuItem ("CosmicChamps/Run/macOS/Release/Secondary", priority = 4)]
        public static void RunSecondaryOSXReleaseInstance ()
        {
            RunInstances (BuildTarget.StandaloneOSX, false, AppIdentifier.StandaloneSecondary);
        }

        [MenuItem ("CosmicChamps/Run/macOS/Release/Both", priority = 4)]
        public static void RunBothOSXReleaseInstance ()
        {
            RunInstances (BuildTarget.StandaloneOSX, false, AppIdentifier.StandalonePrimary, AppIdentifier.StandaloneSecondary);
        }

        [MenuItem ("CosmicChamps/Run/macOS/Debug/Primary", priority = 4)]
        public static void RunPrimaryOSXDebugInstance ()
        {
            RunInstances (BuildTarget.StandaloneOSX, true, AppIdentifier.StandalonePrimary);
        }

        [MenuItem ("CosmicChamps/Run/macOS/Debug/Secondary", priority = 4)]
        public static void RunSecondaryOSXDebugInstance ()
        {
            RunInstances (BuildTarget.StandaloneOSX, true, AppIdentifier.StandaloneSecondary);
        }

        [MenuItem ("CosmicChamps/Run/macOS/Debug/Both", priority = 4)]
        public static void RunBothOSXDebugInstance ()
        {
            RunInstances (BuildTarget.StandaloneOSX, true, AppIdentifier.StandalonePrimary, AppIdentifier.StandaloneSecondary);
        }
    }
}