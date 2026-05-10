using System;
using CosmicChamps.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using Zenject;
using CommandLine = Oddworm.Framework.CommandLine;
using ILogger = Serilog.ILogger;


namespace CosmicChamps.Bootstrap.Server
{
    public class EntryPoint : MonoBehaviour
    {
        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
        [Inject]
        private IScenesLoadingService _scenesLoadingService;

        [Inject]
        private ILogger _logger;

        [Inject]
        private AppProfile _appProfile;

        private async void Start ()
        {
            _logger.Information (
                "BuildInfo {AppVersionString}, App Profile {AppProfile}",
                BuildInfo.AppVersionString,
                _appProfile.Name);

            // Logger.IsMuted = !Debug.isDebugBuild;
            DebugManager.instance.enableRuntimeUI = false;

            Application.runInBackground = true;
            Application.targetFrameRate = 60;

            if (CommandLine.HasKey (CommandLineOptions.ClearAddressables))
            {
                while (!Caching.ready)
                {
                    await UniTask.Yield ();
                }

                Addressables.ClearDependencyCacheAsync (AddressablesLabels.Server);
                await Addressables.CleanBundleCache ();
            }

            await _scenesLoadingService.AppendScene (Scenes.ServerMatchmaking, false);
        }
        #endif
    }
}