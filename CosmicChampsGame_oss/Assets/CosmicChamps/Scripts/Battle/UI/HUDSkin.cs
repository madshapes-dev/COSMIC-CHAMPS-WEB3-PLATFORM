using System;
using UnityEngine;

namespace CosmicChamps.Battle.UI
{
    [Serializable]
    public class HUDSkin
    {
        [SerializeField]
        private string _id;
        
        [SerializeField]
        private Sprite _deckPanelSprite;

        [SerializeField]
        private Sprite _timePanelSprite;

        [SerializeField]
        private Sprite _overtimeTimePanelSprite;

        [SerializeField]
        private Sprite _energyBackground;

        public string ID => _id;

        public Sprite DeckPanelSprite => _deckPanelSprite;

        public Sprite TimePanelSprite => _timePanelSprite;

        public Sprite OvertimeTimePanelSprite => _overtimeTimePanelSprite;

        public Sprite EnergyBackground => _energyBackground;
    }
}