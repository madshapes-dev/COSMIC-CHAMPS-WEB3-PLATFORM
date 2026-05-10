using UnityEngine;
using Zenject;

namespace CosmicChamps.Level
{
    public class RootInstaller : MonoInstaller
    {
        [SerializeField]
        private LevelData _levelData;

        public override void InstallBindings ()
        {
            Container.BindInstance<LevelData> (_levelData);
        }
    }
}