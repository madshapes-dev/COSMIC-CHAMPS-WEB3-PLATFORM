using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CosmicChamps.Battle.Units.UnitComponents.Setups
{
    [Serializable]
    public class AnimatorSetup : UnitComponentSetup<IAnimator>
    {
        #if UNITY_EDITOR
        [CustomPropertyDrawer (typeof (AnimatorSetup))]
        public class AnimatorPropertyDrawer : PropertyDrawer
        {
        }
        #endif
    }
}