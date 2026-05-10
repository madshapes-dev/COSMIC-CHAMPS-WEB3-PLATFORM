namespace CosmicChamps.Battle.Data
{
    public class UnitHp
    {
        public int Value;
        public float NormalizedValue;

        public bool Equals (UnitHp other)
        {
            return Value == other.Value;
        }

        public override bool Equals (object obj)
        {
            return obj is UnitHp other && Equals (other);
        }

        public override int GetHashCode ()
        {
            return Value;
        }
    }
}