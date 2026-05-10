namespace CosmicChamps.Battle.Units.UnitComponents.Abstract
{
    public interface IUnitComponent
    {
        public const string NoId = "CosmicChamps.Battle.UnitComponents.IUnitComponent.NoId";

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        void OnStartServer (IUnit unit)
        {
        }

        void OnStopServer ()
        {
        }

        void Dispose ()
        {
        }
        #endif

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        #endif
    }

    public interface IUnitComponent<TSelf> : IUnitComponent where TSelf : IUnitComponent<TSelf>
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        TSelf Clone ();
        #endif
    }
}