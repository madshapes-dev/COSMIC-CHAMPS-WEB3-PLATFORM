using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CosmicChamps.Battle.Units.UnitComponents.Setups
{
    [Serializable]
    public class AttackSetup : UnitComponentSetup<IAttack>
    {
        #if UNITY_EDITOR
        [CustomPropertyDrawer (typeof (AttackSetup))]
        public class AttackPropertyDrawer : PropertyDrawer
        {
        }
        #endif
    }
}