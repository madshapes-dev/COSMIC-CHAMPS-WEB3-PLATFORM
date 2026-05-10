using System;
using CosmicChamps.Bootstrap.Client.UI;
using Cysharp.Threading.Tasks;
using Zenject;

namespace CosmicChamps
{
    public class ClientScenesLoadingService : IScenesLoadingService
    {
        private readonly ScenesManager _scenesManager;
        private readonly SplashScreenPresenter _splashScreenPresenter;

        private readonly ReactivePropertyProgress progress = new();

        public ClientScenesLoadingService (ScenesManager scenesManager, SplashScreenPresenter splashScreenPresenter)
        {
            _scenesManager = scenesManager;
            _splashScreenPresenter = splashScreenPresenter;
        }

        public async UniTask AppendScene (
            string sceneName,
            bool addressableScene,
            string message = null,
            bool hideSplashScreen = true,
            Action<DiContainer> extraBindings = null)
        {
            message ??= $"Loading {sceneName}";

            _splashScreenPresenter.Display (message, progress);
            await _scenesManager.AppendScene (sceneName, addressableScene, progress, extraBindings);

            if (hideSplashScreen)
                _splashScreenPresenter.Hide ();
        }

        public UniTask AppendScene (
            string sceneName,
            bool addressableScene,
            bool displaySplashScreenButtons,
            string message = null,
            bool hideSplashScreen = true,
            Action<DiContainer> extraBindings = null)
        {
            throw new NotImplementedException ();
        }

        public async UniTask ReplaceScene (
            string sceneName,
            bool addressableScene,
            string message = null,
            bool hideSplashScreen = true)
        {
            message ??= $"Loading {sceneName}";
            _splashScreenPresenter.Display (message, progress);
            await _scenesManager.ReplaceLastScene (sceneName, addressableScene, progress);

            if (hideSplashScreen)
                _splashScreenPresenter.Hide ();
        }

        public async UniTask RemoveAllScenesExceptBootstrap (
            string message = null,
            bool hideSplashScreen = true)
        {
            message ??= "Unloading scenes";
            _splashScreenPresenter.Display (message, progress);
            await _scenesManager.UnloadAllScenesExceptFirst (progress);

            if (hideSplashScreen)
                _splashScreenPresenter.Hide ();
        }
    }
}