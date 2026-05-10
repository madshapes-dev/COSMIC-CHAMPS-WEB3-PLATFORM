using UnityEditor;
using UnityEngine;

namespace CosmicChamps.Editor
{
    public static class EditorRunMenus
    {
        [MenuItem ("CosmicChamps/Run/Editor/Preview", priority = 2)]
        public static void RunPreview ()
        {
            EditorRunMode.IsServer = false;
            PlayModeSceneSetter.SetScene (Scenes.Preview);
            EditorApplication.isPlaying = true;
        }

        [MenuItem ("CosmicChamps/Run/Editor/Client", priority = 0)]
        public static void RunClient ()
        {
            EditorRunMode.IsServer = false;
            if (BuildProfiles.IsCurrentProfileServer ())
                BuildProfiles.SwitchBuildProfile (BuildProfiles.macOS.Debug.Primary);
            PlayModeSceneSetter.SetScene (Scenes.ClientBootstrap);
            EditorApplication.isPlaying = true;
        }

        [MenuItem ("CosmicChamps/Run/Editor/Simulator", priority = 3)]
        public static void RunSimulator ()
        {
            EditorRunMode.IsServer = false;
            if (BuildProfiles.IsCurrentProfileServer ())
                BuildProfiles.SwitchBuildProfile (BuildProfiles.macOS.Debug.Primary);
            PlayModeSceneSetter.SetScene (Scenes.SimulatorBootstrap);
            EditorApplication.isPlaying = true;
        }

        [MenuItem ("CosmicChamps/Run/Editor/Server", priority = 1)]
        public static void RunServer ()
        {
            EditorRunMode.IsServer = true;
            Debug.Log ($"BuildProfiles.IsCurrentProfileServer () {BuildProfiles.IsCurrentProfileServer ()}");
            if (!BuildProfiles.IsCurrentProfileServer ())
                BuildProfiles.SwitchBuildProfile (BuildProfiles.macOS.Debug.Server);
            PlayModeSceneSetter.SetScene (Scenes.ServerBootstrap);
            EditorApplication.isPlaying = true;
        }
    }
}