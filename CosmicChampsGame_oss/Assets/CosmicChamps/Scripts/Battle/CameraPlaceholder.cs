using System;
using CosmicChamps.Battle.Data;
using UnityEngine;

namespace CosmicChamps.Battle
{
    [Serializable]
    public class CameraPlaceholder
    {
        public const string PlayerPositionField = nameof (_playerTeam);

        [SerializeField]
        private PlayerTeam _playerTeam;

        [SerializeField]
        private Transform _placeholder;

        public PlayerTeam PlayerTeam => _playerTeam;
        public Transform Placeholder => _placeholder;
    }
}