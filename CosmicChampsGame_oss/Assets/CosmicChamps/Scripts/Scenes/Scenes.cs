using System.IO;

namespace CosmicChamps
{
    public static class Scenes
    {
        public const string AssetsPath = "Assets/CosmicChamps/Scenes";

        private static string GetPath (string scene) => Path.ChangeExtension (Path.Join (AssetsPath, scene), "unity");

        public const string ClientBootstrap = "ClientBootstrap";
        public const string ServerBootstrap = "ServerBootstrap";
        public const string SimulatorBootstrap = "SimulatorBootstrap";
        public const string HomeScreen = "HomeScreen";
        public const string ServerMatchmaking = "ServerMatchmaking";
        public const string Level = "Level";
        public const string Level2 = "Level2";
        public const string ServerBattle = "ServerBattle";
        public const string ClientBattle = "ClientBattle";
        public const string Preview = "Preview";
    }
}