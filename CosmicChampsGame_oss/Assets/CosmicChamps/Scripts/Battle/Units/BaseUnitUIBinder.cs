using System;
using System.Linq;
using CosmicChamps.Battle.Client;
using CosmicChamps.Battle.Data;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using ThirdParty.Extensions;
using UniRx;
using UnityEngine;
using Zenject;


namespace CosmicChamps.Battle.Units
{
    public class BaseUnitUIBinder : MonoBehaviour
    {
        [Serializable]
        private class HPBarPlaceholder
        {
            [SerializeField]
            private bool _isPlayer;

            [SerializeField]
            private CameraRelativeSide _side;

            [SerializeField]
            private BaseUnitType _type;

            [SerializeField]
            private Transform _placeholder;

            public bool IsPlayer => _isPlayer;

            public CameraRelativeSide Side => _side;

            public BaseUnitType Type => _type;

            public Transform Placeholder => _placeholder;
        }

        [SerializeField]
        private HPBarPlaceholder[] _hpBarPlaceholders;

        [SerializeField]
        private BaseUnitNetworkBehaviour _unit;

        [SerializeField]
        private Transform _playerEmojiPlaceholder;

        [SerializeField]
        private Transform _opponentEmojiPlaceholder;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [InjectOptional]
        private BaseHPPrefabs _hpPrefabs;

        [InjectOptional]
        private BattleService _battleService;

        [InjectOptional]
        private CameraService _cameraService;

        [InjectOptional]
        private Emojis _emojis;

        private BaseUnitHPBar _bar;

        private void Awake ()
        {
            _unit.onStartClient.Subscribe (OnStartClient).AddTo (this);
        }

        private void OnTeam (PlayerTeam playerTeam) => _bar.SetPlayerTeam (playerTeam);

        private void OnHp (UnitHp hp) => _bar.SetValue (hp, true);

        private void OnStartClient (IUnit _)
        {
            var side = _cameraService.GetSide (transform);
            var isPlayer = _unit.OwnerId == _battleService.Player.Id;
            var type = _unit.Type;
            // Logger.LogWarning(this, $"--->>>side {side} isPlayer {isPlayer} type {type}");
            _bar = Instantiate (_hpPrefabs.GetPrefab (isPlayer, type, side), _hpPrefabs.Parent);

            var hpBarPlaceholder = _hpBarPlaceholders.FirstOrDefault (
                x => x.IsPlayer == isPlayer && x.Type.HasFlag (type) && x.Side.HasFlag (side));
            if (hpBarPlaceholder == null)
                throw new InvalidOperationException (
                    $"Unable to find placeholder for isPlayer: {isPlayer}; type: {type}; side: {side}");

            _unit.ObserveEveryValueChanged (x => x.Hp).Subscribe (OnHp).AddTo (this);
            _unit.ObserveEveryValueChanged (x => x.Team).Where (x => x != PlayerTeam.Undefined).Subscribe (OnTeam).AddTo (this);

            _bar.Align (hpBarPlaceholder.Placeholder.position);
            _bar.FadeOut (true);
            _bar.FadeIn ();

            if (type == BaseUnitType.Base)
                (isPlayer ? _emojis.Player : _emojis.Opponent).Align (
                    (isPlayer ? _playerEmojiPlaceholder : _opponentEmojiPlaceholder).position);
        }
        #endif
    }
}