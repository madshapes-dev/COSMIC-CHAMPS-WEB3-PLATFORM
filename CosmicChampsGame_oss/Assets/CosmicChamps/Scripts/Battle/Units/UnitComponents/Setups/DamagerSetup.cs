using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CosmicChamps.Battle.Units.UnitComponents.Setups
{
    [Serializable]
    public class DamagerSetup : UnitComponentSetup<IDamager>
    {
        #if UNITY_EDITOR
        [CustomPropertyDrawer (typeof (DamagerSetup))]
        public class DamagerPropertyDrawer : PropertyDrawer
        {
        }
        #endif
    }
}