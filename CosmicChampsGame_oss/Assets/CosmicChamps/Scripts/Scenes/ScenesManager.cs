using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace CosmicChamps
{
    public class ScenesManager
    {
        public readonly struct Args
        {
            public readonly float BeforeDelay;
            public readonly float AfterDelay;

            public Args (float beforeDelay, float afterDelay)
            {
                BeforeDelay = beforeDelay;
                AfterDelay = afterDelay;
            }
        }

        private readonly AddressablesZenjectSceneLoader _sceneLoader;
        private readonly Args _args;

        public ScenesManager (AddressablesZenjectSceneLoader sceneLoader, Args args)
        {
            _sceneLoader = sceneLoader;
            _args = args;
        }

        public async UniTask AppendScene (
            string sceneName,
            bool addressableScene,
            IProgress<float> progress = null,
            Action<DiContainer> extraBindings = null)
        {
            await UniTask.Delay (TimeSpan.FromSeconds (_args.BeforeDelay));

            if (addressableScene)
                await _sceneLoader
                    .LoadAddressableSceneAsync (sceneName, LoadSceneMode.Additive, extraBindings)
                    .ToUniTask (progress);
            else
                await _sceneLoader
                    .LoadSceneAsync (sceneName, LoadSceneMode.Additive, extraBindings)
                    .ToUniTask (progress);

            SceneManager.SetActiveScene (SceneManager.GetSceneByName (sceneName));
            await UniTask.Delay (TimeSpan.FromSeconds (_args.AfterDelay));
        }

        public async UniTask ReplaceLastScene (string sceneName, bool addressableScene, IProgress<float> progress = null)
        {
            await UniTask.Delay (TimeSpan.FromSeconds (_args.BeforeDelay));
            var scenesCount = SceneManager.sceneCount;
            if (scenesCount > 0)
            {
                var sceneToReplace = SceneManager.GetSceneAt (scenesCount - 1);
                await SceneManager.UnloadSceneAsync (sceneToReplace).ToUniTask (progress);
            }

            if (addressableScene)
                await _sceneLoader
                    .LoadAddressableSceneAsync (sceneName, LoadSceneMode.Additive)
                    .ToUniTask (progress);
            else
                await _sceneLoader
                    .LoadSceneAsync (sceneName, LoadSceneMode.Additive)
                    .ToUniTask (progress);

            SceneManager.SetActiveScene (SceneManager.GetSceneByName (sceneName));
            await UniTask.Delay (TimeSpan.FromSeconds (_args.AfterDelay));
        }

        public async UniTask UnloadAllScenesExceptFirst (IProgress<float> progress = null)
        {
            while (SceneManager.sceneCount > 1)
            {
                var scene = SceneManager.GetSceneAt (SceneManager.sceneCount - 1);
                await SceneManager
                    .UnloadSceneAsync (scene)
                    .ToUniTask (progress);
            }
        }
    }
}