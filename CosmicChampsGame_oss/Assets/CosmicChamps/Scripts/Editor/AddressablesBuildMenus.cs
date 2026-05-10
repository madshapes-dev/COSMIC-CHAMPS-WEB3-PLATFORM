using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Profile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CosmicChamps.Editor
{
    public class AddressablesBuildMenus
    {
        private static readonly Dictionary<BuildTarget, AddressablesPlatform> _buildTargetMapping =
            new()
            {
                { BuildTarget.XboxOne, AddressablesPlatform.XboxOne },
                { BuildTarget.Switch, AddressablesPlatform.Switch },
                { BuildTarget.PS4, AddressablesPlatform.PS4 },
                { BuildTarget.iOS, AddressablesPlatform.iOS },
                { BuildTarget.Android, AddressablesPlatform.Android },
                { BuildTarget.WebGL, AddressablesPlatform.WebGL },
                { BuildTarget.StandaloneWindows, AddressablesPlatform.Windows },
                { BuildTarget.StandaloneWindows64, AddressablesPlatform.Windows },
                { BuildTarget.StandaloneOSX, AddressablesPlatform.OSX },
                { BuildTarget.StandaloneLinux64, AddressablesPlatform.Linux },
                { BuildTarget.WSAPlayer, AddressablesPlatform.WindowsUniversal },
            };

        private static void BuildAddressables (BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
        {
            Debug.Log ($"[Addressables Build] Switch target to {buildTargetGroup} ({buildTarget})...");
            BuildProfile.SetActiveBuildProfile (null);
            EditorUserBuildSettings.SwitchActiveBuildTarget (buildTargetGroup, buildTarget);
            Debug.Log ("[Addressables Build] BuildPlayerContent...");
            AddressableAssetSettings.BuildPlayerContent ();
            Debug.Log ("[Addressables Build] Done");
        }

        private static void UpdateAddressables (BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
        {
            Debug.Log ($"[Addressables Update] Switch target to to {buildTargetGroup} ({buildTarget})...");
            
            BuildProfile.SetActiveBuildProfile (null);
            EditorUserBuildSettings.SwitchActiveBuildTarget (buildTargetGroup, buildTarget);

            var path = Path.Combine (
                Application.dataPath,
                "AddressableAssetsData",
                _buildTargetMapping[EditorUserBuildSettings.activeBuildTarget].ToString (),
                "addressables_content_state.bin");

            var guids = AssetDatabase.FindAssets ("t:AddressableAssetSettings");
            Debug.Log (
                $"[Addressables Update] guids.Length {guids.Length} path {AssetDatabase.GUIDToAssetPath (guids[0])}");
            var buildSettings =
                AssetDatabase.LoadAssetAtPath<AddressableAssetSettings> (AssetDatabase.GUIDToAssetPath (guids[0]));

            ContentUpdateScript.BuildContentUpdate (buildSettings, path);

            Debug.Log ("[Addressables Update] Done");            
        }

        [MenuItem ("CosmicChamps/Addressables/Build/macOS", priority = 7)]
        public static void BuildClientMacOSPrimaryAddressables ()
        {
            BuildAddressables (BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
        }

        [MenuItem ("CosmicChamps/Addressables/Build/Linux", priority = 7)]
        public static void BuildServerLinuxAddressables ()
        {
            BuildAddressables (BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64);
        }

        [MenuItem ("CosmicChamps/Addressables/Build/Android", priority = 7)]
        public static void BuildClientAndroidAddressables ()
        {
            BuildAddressables (BuildTargetGroup.Android, BuildTarget.Android);
        }

        [MenuItem ("CosmicChamps/Addressables/Build/iOS", priority = 7)]
        public static void BuildClientIOSAddressables ()
        {
            BuildAddressables (BuildTargetGroup.iOS, BuildTarget.iOS);
        }

        [MenuItem ("CosmicChamps/Addressables/Build/WebGL", priority = 7)]
        public static void BuildClientWebGLAddressables ()
        {
            BuildAddressables (BuildTargetGroup.WebGL, BuildTarget.WebGL);
        }

        [MenuItem ("CosmicChamps/Addressables/Update/macOS", priority = 7)]
        public static void UpdateClientMacOSAddressables ()
        {
            UpdateAddressables (BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
        }

        [MenuItem ("CosmicChamps/Addressables/Update/Linux", priority = 7)]
        public static void UpdateServerLinuxAddressables ()
        {
            UpdateAddressables (BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64);
        }

        [MenuItem ("CosmicChamps/Addressables/Update/Android", priority = 7)]
        public static void UpdateClientAndroidAddressables ()
        {
            UpdateAddressables (BuildTargetGroup.Android, BuildTarget.Android);
        }

        [MenuItem ("CosmicChamps/Addressables/Update/iOS", priority = 7)]
        public static void UpdateClientIOSAddressables ()
        {
            UpdateAddressables (BuildTargetGroup.iOS, BuildTarget.iOS);
        }

        [MenuItem ("CosmicChamps/Addressables/Update/WebGL", priority = 7)]
        public static void UpdateClientWebGLAddressables ()
        {
            UpdateAddressables (BuildTargetGroup.WebGL, BuildTarget.WebGL);
        }

        [MenuItem ("CosmicChamps/Addressables/Clear Cache", priority = 8)]
        public static void BuildAndroidDebug ()
        {
            Caching.ClearCache ();
        }
    }
}