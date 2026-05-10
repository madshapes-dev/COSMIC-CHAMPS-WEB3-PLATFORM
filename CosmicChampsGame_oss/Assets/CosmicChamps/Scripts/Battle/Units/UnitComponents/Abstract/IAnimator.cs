namespace CosmicChamps.Battle.Units.UnitComponents.Abstract
{
    public interface IAnimator : IUnitComponent<IAnimator>
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        void Move ();
        void Stand ();
        void Attack ();
        void Die ();
        void Deploy ();
        void SetMovementSpeed (float speed);
        #endif
    }
}