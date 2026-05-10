using UnityEditor;
using UnityEditor.SceneManagement;

namespace CosmicChamps.Editor
{
    [InitializeOnLoad]
    public static class PlayModeSceneSetter
    {
        private const string PlayModeStartScene = "PlayModeStartScene";

        static PlayModeSceneSetter ()
        {
            SetScene (
                EditorPrefs.HasKey (PlayModeStartScene)
                    ? EditorPrefs.GetString (PlayModeStartScene)
                    : Scenes.ClientBootstrap);
        }

        public static void SetScene (string scene)
        {
            var launcherGuid = AssetDatabase.FindAssets ($"t:scene {scene}")[0];
            var launcherPath = AssetDatabase.GUIDToAssetPath (launcherGuid);
            var launcherAsset = AssetDatabase.LoadAssetAtPath<SceneAsset> (launcherPath);

            EditorSceneManager.playModeStartScene = launcherAsset;
            EditorPrefs.SetString (PlayModeStartScene, scene);
        }
    }
}