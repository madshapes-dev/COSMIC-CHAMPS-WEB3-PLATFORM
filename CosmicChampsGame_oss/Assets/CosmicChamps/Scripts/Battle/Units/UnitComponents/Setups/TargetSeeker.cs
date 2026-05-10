using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CosmicChamps.Battle.Units.UnitComponents.Setups
{
    [Serializable]
    public class TargetSeekerSetup : UnitComponentSetup<ITargetSeeker>
    {
        #if UNITY_EDITOR
        [CustomPropertyDrawer (typeof (TargetSeekerSetup))]
        public class TargetSeekerPropertyDrawer : PropertyDrawer
        {
        }
        #endif
    }
}