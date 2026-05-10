using System;
using CosmicChamps.Battle;
using CosmicChamps.Battle.Data;
using CosmicChamps.Data;
using ThirdParty.Extensions.Attributes;
using UnityEngine;

namespace CosmicChamps.Level
{
    public class LevelData : MonoBehaviour
    {
        [Serializable]
        public class Cannon
        {
            public const string TypeField = nameof (_type);

            [SerializeField]
            private BaseUnitType _type;

            [SerializeField]
            private Transform _pivot;

            [SerializeField]
            private Transform _muzzle;

            public BaseUnitType Type => _type;

            public Transform Pivot => _pivot;

            public Transform Muzzle => _muzzle;
        }

        [Serializable]
        public struct Base
        {
            public const string PositionField = nameof (_playerTeam);

            [SerializeField]
            private PlayerTeam _playerTeam;

            [SerializeField]
            private Quaternion _spawnRotation;

            [SerializeField]
            private ShipFlyController _shipFlyController;

            [SerializeField]
            private Transform _basePlaceholder;

            [SerializeField]
            private Transform _turretPlaceholder;

            [SerializeField]
            private Transform _shieldPlaceholder;

            [SerializeField]
            private SpawnArea[] _spawnAreas;

            [SerializeField]
            private Cannon[] _cannons;

            public PlayerTeam PlayerTeam => _playerTeam;

            public Quaternion SpawnRotation => _spawnRotation;

            public ShipFlyController ShipFlyController => _shipFlyController;

            public Transform BasePlaceholder => _basePlaceholder;

            public Transform TurretPlaceholder => _turretPlaceholder;

            public Transform ShieldPlaceholder => _shieldPlaceholder;

            public Cannon[] Cannons => _cannons;

            public SpawnArea[] SpawnAreas => _spawnAreas;

            public SpawnArea GetSpawnArea (string spawnArea) => Array.Find (_spawnAreas, x => x.Type == spawnArea);
        }

        [SerializeField, ArrayElementTitle (Base.PositionField)]
        private Base[] _bases;

        [SerializeField]
        private SpellArea _spellArea;

        public SpellArea SpellArea => _spellArea;
        public Base[] Bases => _bases;
        public Base GetBase (PlayerTeam team) => Array.Find (_bases, x => x.PlayerTeam == team);
    }
}