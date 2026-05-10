namespace CosmicChamps
{
    #if UNITY_EDITOR
    public class EditorRunMode
    {
        private const string Key = nameof (EditorRunMode);

        public static bool IsServer
        {
            get => UnityEditor.EditorPrefs.GetBool (Key, false);
            set => UnityEditor.EditorPrefs.SetBool (Key, value);
        }
    }
    #endif
}