using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CosmicChamps.Battle.Units.UnitComponents.Setups
{
    [Serializable]
    public class DeathSetup : UnitComponentSetup<IDeath>
    {
        #if UNITY_EDITOR
        [CustomPropertyDrawer (typeof (DeathSetup))]
        public class DeathPropertyDrawer : PropertyDrawer
        {
        }
        #endif
    }
}