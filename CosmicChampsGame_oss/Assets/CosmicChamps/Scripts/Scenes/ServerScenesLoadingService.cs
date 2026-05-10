using System;
using Cysharp.Threading.Tasks;
using Zenject;

namespace CosmicChamps
{
    public class ServerScenesLoadingService : IScenesLoadingService
    {
        private readonly ScenesManager _scenesManager;

        public ServerScenesLoadingService (ScenesManager scenesManager)
        {
            _scenesManager = scenesManager;
        }

        public async UniTask AppendScene (
            string sceneName,
            bool addressableScene,
            string message = null,
            bool hideSplashScreen = true,
            Action<DiContainer> extraBindings = null) =>
            //
            await _scenesManager.AppendScene (sceneName, addressableScene, null, extraBindings);

        public async UniTask ReplaceScene (
            string sceneName,
            bool addressableScene,
            string message = null,
            bool hideSplashScreen = true) =>
            //
            await _scenesManager.ReplaceLastScene (sceneName, addressableScene);

        public async UniTask RemoveAllScenesExceptBootstrap (
            string message = null,
            bool hideSplashScreen = true) =>
            //
            await _scenesManager.UnloadAllScenesExceptFirst ();
    }
}