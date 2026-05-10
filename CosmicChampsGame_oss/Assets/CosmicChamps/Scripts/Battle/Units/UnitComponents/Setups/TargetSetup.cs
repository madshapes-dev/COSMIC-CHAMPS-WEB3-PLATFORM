using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CosmicChamps.Battle.Units.UnitComponents.Setups
{
    [Serializable]
    public class TargetSetup : UnitComponentSetup<ITarget>
    {
        #if UNITY_EDITOR
        [CustomPropertyDrawer (typeof (TargetSetup))]
        public class TargetPropertyDrawer : PropertyDrawer
        {
        }
        #endif
    }
}