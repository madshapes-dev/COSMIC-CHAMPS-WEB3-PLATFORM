using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CosmicChamps.Battle.Units.UnitComponents.Setups
{
    [Serializable]
    public class MovementSetup : UnitComponentSetup<IMovement>
    {
        #if UNITY_EDITOR
        [CustomPropertyDrawer (typeof (MovementSetup))]
        public class MovementPropertyDrawer : PropertyDrawer
        {
        }
        #endif
    }
}