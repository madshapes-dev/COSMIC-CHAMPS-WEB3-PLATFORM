using UnityEngine;

namespace CosmicChamps.Data
{
    public class BoostedUnitStats : IUnitStats
    {
        private readonly IUnitStats _unitStats;
        private readonly UnitBoost _unitBoost;

        public BoostedUnitStats (IUnitStats unitStats, UnitBoost unitBoost)
        {
            _unitStats = unitStats;
            _unitBoost = unitBoost;

            Hp = _unitBoost.Hp.HasValue
                ? _unitStats.Hp.Boost (_unitBoost.Hp.Value)
                : _unitStats.Hp;
            Damage = _unitBoost.Damage.HasValue
                ? _unitStats.Damage.Boost (_unitBoost.Damage.Value)
                : _unitStats.Damage;
            DeathDamage = _unitBoost.DeathDamage.HasValue
                ? _unitStats.DeathDamage.Boost (_unitBoost.DeathDamage.Value)
                : _unitStats.DeathDamage;

            Debug.Log ($"_unitStats.Hp {_unitStats.Hp[0]} Hp {Hp[0]}");
            Debug.Log ($"_unitStats.Damage {_unitStats.Damage[0]} Damage {Damage[0]}");
        }

        public Hp Hp { get; }
        public Damage Damage { get; }
        public Damage DeathDamage { get; }
        public float Speed => _unitStats.Speed;
        public float DetectRange => _unitStats.DetectRange;

        public IUnitStats Clone () => new BoostedUnitStats (_unitStats, _unitBoost);
    }
}