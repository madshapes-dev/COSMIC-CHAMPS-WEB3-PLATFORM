using System;
using UnityEngine;

namespace CosmicChamps.Battle.UI
{
    [CreateAssetMenu (menuName = "Cosmic Champs/HUD Skins")]
    public class HUDSkins : ScriptableObject
    {
        [SerializeField]
        private HUDSkin _fallbackSkin;

        [SerializeField]
        private HUDSkin[] _skins;

        public HUDSkin GetSkin (string id) => Array.Find (_skins, x => x.ID == id) ?? _fallbackSkin;
    }
}