using CosmicChamps.Services;
using UnityEditor;

namespace CosmicChamps.Editor
{
    public static class PlayerPrefsMenus
    {
        private static PlayerPrefsService _playerPrefsService;

        [MenuItem ("CosmicChamps/Player Prefs/Clear Tokens")]
        public static void Clear ()
        {
            _playerPrefsService ??= new PlayerPrefsService ();
            _playerPrefsService.Tokens.Clear ();
        }
    }
}