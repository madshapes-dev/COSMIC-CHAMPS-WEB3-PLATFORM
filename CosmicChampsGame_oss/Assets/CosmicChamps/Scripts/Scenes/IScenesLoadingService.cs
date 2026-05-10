using System;
using Cysharp.Threading.Tasks;
using Zenject;

namespace CosmicChamps
{
    public interface IScenesLoadingService
    {
        UniTask AppendScene (
            string sceneName,
            bool addressableScene,
            string message = null,
            bool hideSplashScreen = true,
            Action<DiContainer> extraBindings = null);

        UniTask ReplaceScene (
            string sceneName,
            bool addressableScene,
            string message = null,
            bool hideSplashScreen = true);

        UniTask RemoveAllScenesExceptBootstrap (
            string message = null,
            bool hideSplashScreen = true);
    }
}