using System;
using CosmicChamps.Settings;
using UnityEditor;
using Zenject;

namespace CosmicChamps.Editor
{
    public static class AppProfileMenus
    {
        /*private const string AddressablesVarsTemplate = @"namespace CosmicChamps
{
    public static class AddressablesVars
    {
        public const string RemoteLoadUrl = ""{REMOTE_LOAD_PATH}"";
    }
}";*/

        public static RootInstaller GetRootInstaller ()
        {
            var guids = AssetDatabase.FindAssets ("t:prefab ProjectContext");
            if (guids.Length == 0)
                throw new InvalidOperationException ("Unable to find ProjectContext");

            var projectContext = AssetDatabase.LoadAssetAtPath<ProjectContext> (AssetDatabase.GUIDToAssetPath (guids[0]));
            var rootInstaller = projectContext.GetComponent<RootInstaller> ();
            if (rootInstaller == null)
                throw new InvalidOperationException ("No root installer attached to ProjectContext");

            return rootInstaller;
        }

        /*private static void UpdateAddressablesVars (AppProfile appProfile)
        {
            const string scriptPath = "CosmicChamps/Scripts/Addressables/AddressablesVars.cs";
            File.WriteAllText (
                $"{Application.dataPath}/{scriptPath}",
                AddressablesVarsTemplate.Replace ("{REMOTE_LOAD_PATH}", appProfile.BundlesLoadUrl));
            AssetDatabase.ImportAsset ($"Assets/{scriptPath}", ImportAssetOptions.ForceUpdate);
        }*/

        private static void ApplyProfile (string profileName)
        {
            var appProfile = GetProfile (profileName);
            if (appProfile == null)
                return;

            // UpdateAddressablesVars (appProfile);

            var rootInstaller = GetRootInstaller ();
            var serializedObject = new SerializedObject (rootInstaller);
            serializedObject
                .FindProperty (RootInstaller.AppProfileField)
                .objectReferenceValue = appProfile;

            serializedObject.ApplyModifiedPropertiesWithoutUndo ();
            AssetDatabase.SaveAssets ();
        }

        private static AppProfile GetProfile (string profileName)
        {
            if (string.IsNullOrEmpty (profileName))
                return null;

            var guids = AssetDatabase.FindAssets ($"t:{nameof (AppProfile)} {profileName}");
            if (guids.Length == 0)
                return null;

            var appProfile = AssetDatabase.LoadAssetAtPath<AppProfile> (AssetDatabase.GUIDToAssetPath (guids[0]));
            return appProfile;
        }

        [MenuItem ("CosmicChamps/App Profile/Dev", priority = 20)]
        public static void ApplyDevProfile () => ApplyProfile ("DevAppProfile");

        [MenuItem ("CosmicChamps/App Profile/Test A", priority = 20)]
        public static void ApplyTestAProfile () => ApplyProfile ("TestAAppProfile");

        [MenuItem ("CosmicChamps/App Profile/Test B", priority = 20)]
        public static void ApplyTestBProfile () => ApplyProfile ("TestBAppProfile");

        [MenuItem ("CosmicChamps/App Profile/Local", priority = 20)]
        public static void ApplyLocalProfile () => ApplyProfile ("LocalAppProfile");

        [MenuItem ("CosmicChamps/App Profile/Localhost", priority = 20)]
        public static void ApplyLocalhostProfile () => ApplyProfile ("LocalhostAppProfile");
    }
}