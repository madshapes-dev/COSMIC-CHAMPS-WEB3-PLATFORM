using System;
using UnityEngine;

namespace CosmicChamps.Battle.Data
{
    public class PlayerEnergy
    {
        private readonly int _maxValue;
        private readonly Func<float> _timeProvider;

        private float _value;
        private float _timestamp;
        private float _growRate;

        public float Value => Mathf.Min (_maxValue, _value + _growRate * (_timeProvider () - _timestamp));
        public float NormalizedValue => Value / _maxValue;
        public float GrowRate => _growRate;
        public int MaxValue => _maxValue;

        public PlayerEnergy (float value, float growRate, int maxValue, Func<float> timeProvider)
        {
            _maxValue = maxValue;
            _timeProvider = timeProvider;
            _value = value;
            _growRate = growRate;
            _timestamp = _timeProvider ();
        }

        private void SetValue (float value)
        {
            _value = value;
            _timestamp = _timeProvider ();
        }

        public void Reinitialize (float value, float growRate)
        {
            SetValue (value);
            _growRate = growRate;
        }

        public void Spend (float spendValue)
        {
            var currentValue = Value;
            if (spendValue > currentValue)
                throw new InvalidOperationException ("Not enough energy");

            SetValue (currentValue - spendValue);
        }

        public void SetGrowRate (float growRate)
        {
            SetValue (Value);
            _growRate = growRate;
        }
    }
}