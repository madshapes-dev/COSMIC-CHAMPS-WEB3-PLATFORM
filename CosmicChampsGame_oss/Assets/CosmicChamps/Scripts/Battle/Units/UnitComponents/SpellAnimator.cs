using System;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;

namespace CosmicChamps.Battle.Units.UnitComponents
{
    [Serializable]
    public class SpellAnimator : IAnimator
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        public IAnimator Clone () => new SpellAnimator ();

        public void Move ()
        {
        }

        public void Stand ()
        {
        }

        public void Attack ()
        {
        }

        public void Die ()
        {
        }

        public void Deploy ()
        {
        }

        public void SetMovementSpeed (float speed)
        {
        }
        #endif
    }
}