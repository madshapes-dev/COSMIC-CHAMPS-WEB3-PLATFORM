namespace CosmicChamps.Data
{
    public class UnitStats : IUnitStats
    {
        public static readonly IUnitStats Empty = new UnitStats
        {
            Hp = null,
            Damage = null,
            DeathDamage = null,
            Speed = 0,
            DetectRange = 0
        };

        public Hp Hp { set; get; }
        public Damage Damage { set; get; }
        public Damage DeathDamage { set; get; }
        public float Speed { set; get; }
        public float DetectRange { set; get; }

        public IUnitStats Clone () => new UnitStats
        {
            Hp = Hp?.Clone (),
            Damage = Damage?.Clone (),
            DeathDamage = DeathDamage?.Clone (),
            Speed = Speed,
            DetectRange = DetectRange
        };
    }
}