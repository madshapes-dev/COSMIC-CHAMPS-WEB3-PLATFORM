using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using CosmicChamps.Data;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class DummyDamager : IDamager
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        public void ApplyDamage (Damage damage, int damagerLevel)
        {
        }

        public IDamager Clone () => new DummyDamager ();
        #endif
    }
}