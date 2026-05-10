using System;
using CosmicChamps.Battle.UI;
using UnityEngine;

namespace CosmicChamps.Battle
{
    [Serializable]
    public class Emojis
    {
        [SerializeField]
        private PlayerEmoji _player;

        [SerializeField]
        private PlayerEmoji _opponent;

        public PlayerEmoji Player => _player;

        public PlayerEmoji Opponent => _opponent;
    }
}