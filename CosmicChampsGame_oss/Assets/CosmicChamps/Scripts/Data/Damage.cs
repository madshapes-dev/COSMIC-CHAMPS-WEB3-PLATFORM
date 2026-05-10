using System;
using UnityEngine;

namespace CosmicChamps.Data
{
    public class Damage
    {
        private readonly UnitTargetType _splashVictims;
        private readonly int[] _values;

        public float Rate { get; }
        public float Range { get; }
        public UnitTargetType Triggers { get; }
        public UnitTargetType SplashVictims => _splashVictims != UnitTargetType.None ? _splashVictims : Triggers;

        public Damage (UnitTargetType splashVictims, int[] values, float rate, float range, UnitTargetType triggers)
        {
            _splashVictims = splashVictims;
            _values = values;
            Rate = rate;
            Range = range;
            Triggers = triggers;
        }

        public int this [int level]
        {
            get
            {
                if (level < _values.Length)
                    return _values[level];

                throw new ArgumentException ($"No Value data for level {level}");
            }
        }

        public Damage Clone () => new(
            _splashVictims,
            _values,
            Rate,
            Range,
            Triggers);

        public Damage Boost (int value) => new(
            _splashVictims,
            Array.ConvertAll (_values, x => Mathf.RoundToInt (x * (1f + value / 100f))),
            Rate,
            Range,
            Triggers);
    }
}