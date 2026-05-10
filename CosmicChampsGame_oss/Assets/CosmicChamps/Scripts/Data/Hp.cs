using System;
using UnityEngine;

namespace CosmicChamps.Data
{
    public class Hp
    {
        private readonly int[] _values;

        public Hp (int[] values)
        {
            _values = values;
        }

        public int this [int level]
        {
            get
            {
                if (level < _values.Length)
                    return _values[level];

                throw new ArgumentException ($"No Hp data for level {level}");
            }
        }

        public Hp Clone () => new(_values);

        public Hp Boost (int value) => new(Array.ConvertAll (_values, x => Mathf.RoundToInt (x * (1f + value / 100f))));

        protected bool Equals (Hp other)
        {
            return Equals (_values, other._values);
        }

        public override bool Equals (object obj)
        {
            if (ReferenceEquals (null, obj)) return false;
            if (ReferenceEquals (this, obj)) return true;
            if (obj.GetType () != this.GetType ()) return false;
            return Equals ((Hp)obj);
        }

        public override int GetHashCode ()
        {
            return (_values != null ? _values.GetHashCode () : 0);
        }
    }
}