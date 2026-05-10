namespace CosmicChamps.Data
{
    public interface IUnitStats
    {
        Hp Hp { get; }
        Damage Damage { get; }
        Damage DeathDamage { get; }
        float Speed { get; }
        float DetectRange { get; }

        IUnitStats Clone ();
    }
}