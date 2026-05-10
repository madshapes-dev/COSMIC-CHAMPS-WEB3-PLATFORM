using CosmicChamps.Battle.Data;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using UniRx;
using UnityEngine;
using Zenject;

namespace CosmicChamps.Battle.Units
{
    public class RegularUnitHpBarBinder : MonoBehaviour
    {
        [SerializeField]
        private RegularUnitNetworkBehaviour _unit;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [InjectOptional]
        private UnitHPBarFactory _barFactory;

        private AbstractUnitHPBar _bar;

        private void Awake ()
        {
            _unit.onStartClient.Subscribe (OnStartClient).AddTo (this);
        }

        private void OnStartClient (IUnit _)
        {
            _bar = _barFactory.Create ();

            _unit
                .ObserveEveryValueChanged (x => x.Hp)
                .Subscribe (OnHp)
                .AddTo (this);

            _unit
                .ObserveEveryValueChanged (x => x.Team)
                .Where (x => x != PlayerTeam.Undefined)
                .Subscribe (OnTeam)
                .AddTo (this);

            _unit
                .ObserveEveryValueChanged (x => x.Level)
                .Subscribe (OnLevel)
                .AddTo (this);
        }

        private void OnLevel (int level) => _bar.SetLevel(level);

        private void Update ()
        {
            if (_bar != null)
                _bar.Align (_unit.BarPosition);
        }

        private void OnDestroy ()
        {
            if (_bar != null)
                _bar.Dispose ();
        }

        private void OnTeam (PlayerTeam playerTeam) => _bar.SetPlayerTeam (playerTeam);

        private void OnHp (UnitHp hp) => _bar.SetValue (hp, true);
        #endif
    }
}