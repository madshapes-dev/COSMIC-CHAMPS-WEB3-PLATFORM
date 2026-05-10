using CosmicChamps.Data;

namespace CosmicChamps.Battle.Units.UnitComponents.Abstract
{
    public interface IDamager : IUnitComponent<IDamager>
    {
        public const string Default = "CosmicChamps.Battle.UnitComponents.IDamager.Default";
        public const string Death = "CosmicChamps.Battle.UnitComponents.IDamager.Death";

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        void ApplyDamage (Damage damage, int damagerLevel);
        #endif
    }
}