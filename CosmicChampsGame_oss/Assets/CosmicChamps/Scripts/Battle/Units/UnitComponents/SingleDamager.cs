using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using CosmicChamps.Data;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class SingleDamager : IDamager
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        private ITargetSeeker _targetSeeker;

        public void ApplyDamage (Damage damage, int damagerLevel) => _targetSeeker
            .GetCurrentTarget ()
            ?.ApplyDamage (damage[damagerLevel]);

        public IDamager Clone () => new SingleDamager ();

        public void OnStartServer (IUnit unit)
        {
            _targetSeeker = unit.GetUnitComponent<ITargetSeeker> ();
        }
        #endif
    }
}