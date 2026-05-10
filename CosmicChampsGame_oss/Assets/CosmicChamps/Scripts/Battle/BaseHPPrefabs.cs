using System;
using System.Linq;
using UnityEngine;

namespace CosmicChamps.Battle
{
    [Serializable]
    public class BaseHPPrefabs
    {
        [Serializable]
        private class BaseHPPrefab
        {
            [SerializeField]
            private bool _isPlayer;

            [SerializeField]
            private CameraRelativeSide _side;

            [SerializeField]
            private BaseUnitType _type;

            [SerializeField]
            private BaseUnitHPBar _prefab;

            public bool IsPlayer => _isPlayer;

            public BaseUnitHPBar Prefab => _prefab;

            public CameraRelativeSide Side => _side;

            public BaseUnitType Type => _type;
        }

        [SerializeField]
        private BaseHPPrefab[] _prefabs;

        [SerializeField]
        private Transform _parent;

        public Transform Parent => _parent;

        public BaseUnitHPBar GetPrefab (bool isPlayer, BaseUnitType type, CameraRelativeSide side)
        {
            var prefab = _prefabs.FirstOrDefault (x => x.IsPlayer == isPlayer && x.Type.HasFlag (type) && x.Side.HasFlag (side));
            if (prefab == null)
                throw new ArgumentOutOfRangeException (
                    $"Cannot find BaseUnitHPBar prefab for isPlayer: {isPlayer}; type: {type}; side: {side}");

            return prefab.Prefab;
        }
    }
}