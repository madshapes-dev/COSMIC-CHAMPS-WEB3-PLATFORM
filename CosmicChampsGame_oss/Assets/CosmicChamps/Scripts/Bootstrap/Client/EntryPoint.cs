using System;
using System.Collections.Generic;
using System.Threading;
using CosmicChamps.Bootstrap.Client.UI;
using CosmicChamps.Common;
using CosmicChamps.Services;
using CosmicChamps.Settings;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using CosmicChamps.Utils;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using ThirdParty.Extensions.CanvasGroupFader;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.UI;
using Zenject;
using Addressables = UnityEngine.AddressableAssets.Addressables;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Bootstrap.Client
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        private LocalizedString _loadingMessage;

        [SerializeField]
        private LocalizedString _restartMessage;

        [SerializeField]
        private LocalizedString _bundlesUpdateCheckMessage;

        [SerializeField]
        private LocalizedString _bundlesUpdateMessage;

        [Header ("Clear Cache"), SerializeField]
        private RectTransform _buttonsBlock;

        [SerializeField]
        private Button _clearCacheButton;

        [SerializeField]
        private ClearCachePopup _clearCachePopup;

        [SerializeField]
        private LocalizedString _clearCacheTimeoutMessage;

        [SerializeField]
        private LocalizedString _clearCacheMessage;

        [SerializeField]
        private float _clearCacheTime = 4f;

        [Inject]
        private IScenesLoadingService _scenesLoadingService;

        [Inject]
        private IMessageBroker _messageBroker;

        [Inject]
        private List<IRestartListener> _restartListeners;

        [Inject]
        private SplashScreenPresenter _splashScreenPresenter;

        [Inject]
        private UpdateHandlingService _updateHandlingService;

        [Inject]
        private NetworkReachableReporterProgress.Factory _networkReachableReporterProgressFactory;

        [Inject]
        private NetworkingConfig _networkingConfig;

        [Inject]
        private MutableWalletBridgeProvider _mutableWalletBridgeProvider;

        [Inject]
        private ILogger _logger;

        [Inject]
        private IBrandingFactory _brandingFactory;

        [Inject]
        private IImmutableService _immutableService;

        [Inject]
        private PlayerPrefsService _playerPrefsService;

        [Inject]
        private UILocker _uiLocker;
        
        [Inject]
        private IShardsViewProvider _shardsViewProvider;

        private int _updateAddressablesAttempts;
        private CancellationTokenSource _clearCahceCancellation;

        private void Awake ()
        {
            _clearCacheButton
                .OnClickAsObservable ()
                .Subscribe (OnClearCacheButtonClicked)
                .AddTo (this);

            _messageBroker
                .Receive<RestartSignal> ()
                .Subscribe (OnRestart)
                .AddTo (this);
        }

        private async void Start ()
        {
            DebugManager.instance.enableRuntimeUI = false;

            Application.runInBackground = true;
            Application.targetFrameRate = 60;

            /*#if !UNITY_WEBGL
            var clearAddressables = CommandLine.HasKey (CommandLineOptions.ClearAddressables);
            _logger.Information ("clearAddressables {ClearAddressables}", clearAddressables);
            if (clearAddressables)
                await ClearAddressables ();

            var clearPrefs = CommandLine.HasKey (CommandLineOptions.ClearPrefs);
            _logger.Information ("clearPrefs {ClearPrefs}", clearPrefs);
            if (clearPrefs)
                _playerPrefsService.ClearAll ();
            #endif*/

            #if UNITY_WEBGL
            _buttonsBlock.FadeOut (true);
            await Bootstrap ();
            #else
            _clearCahceCancellation = new CancellationTokenSource ();
            try
            {
                _splashScreenPresenter.Display (await _clearCacheTimeoutMessage.GetLocalizedStringAsync ());
                await UniTask.Delay (TimeSpan.FromSeconds (_clearCacheTime), cancellationToken: _clearCahceCancellation.Token);
                _clearCahceCancellation = null;
                _buttonsBlock.FadeOut ();

                await Bootstrap ();
            } catch (OperationCanceledException)
            {
            }
            #endif
        }

        private void OnClearCacheButtonClicked (Unit _)
        {
            if (_clearCahceCancellation == null)
                return;

            _clearCahceCancellation.Cancel ();
            _clearCahceCancellation = null;
            _buttonsBlock.FadeOut ();

            _clearCachePopup.Display (OnClearCachePopupYesClicked, OnClearCachePopupNoClicked);
        }

        private void OnClearCachePopupYesClicked ()
        {
            async UniTaskVoid BootstrapAfterClearCache ()
            {
                _clearCachePopup.Hide ();
                _splashScreenPresenter.Display (await _clearCacheMessage.GetLocalizedStringAsync ());

                _playerPrefsService.ClearAll ();
                await ClearAddressables ();
                await Bootstrap ();
            }

            BootstrapAfterClearCache ().Forget ();
        }

        private void OnClearCachePopupNoClicked ()
        {
            _clearCachePopup.Hide ();
            Bootstrap ().Forget ();
        }

        private async UniTask Bootstrap ()
        {
            _splashScreenPresenter.Display (await _bundlesUpdateCheckMessage.GetLocalizedStringAsync ());

            _mutableWalletBridgeProvider.WalletBridgeUrl = await _updateHandlingService.PerformVersionCheck ();
            _updateAddressablesAttempts = 0;

            await UpdateBundles ();
            await _shardsViewProvider.SetDefaultTMPSprites ();
            await _scenesLoadingService.AppendScene (
                Scenes.HomeScreen,
                false,
                await _loadingMessage.GetLocalizedStringAsync (),
                false);
        }

        private async UniTask ClearAddressables ()
        {
            #if !UNITY_WEBGL
            while (!Caching.ready)
            {
                await UniTask.Yield ();
            }
            #endif

            await Addressables.ClearDependencyCacheAsync (AddressablesLabels.Client, true);
            await Addressables.CleanBundleCache ();
        }

        private async UniTask UpdateBundles ()
        {
            try
            {
                _logger.Information ("UnloadUnusedAssets...");
                await Resources.UnloadUnusedAssets ();
                _logger.Information ("UnloadUnusedAssets Done");

                _logger.Information ("CheckForCatalogUpdates...");
                var catalogIds = await Addressables
                    .CheckForCatalogUpdates ()
                    .ToUniTask (_networkReachableReporterProgressFactory.Create ());
                _logger.Information ("CheckForCatalogUpdates Done: {Join}", string.Join (", ", catalogIds));
                if (catalogIds.Count > 0)
                {
                    _logger.Information ("UpdateCatalogs...");
                    await Addressables
                        .UpdateCatalogs (true, catalogIds)
                        .ToUniTask (_networkReachableReporterProgressFactory.Create ());
                    _logger.Information ("UpdateCatalogs Done");
                } else
                {
                    await UniTask.Delay (TimeSpan.FromSeconds (1f));
                }

                _logger.Information ("GetDownloadSize...");
                var getDownloadSize = await Addressables
                    .GetDownloadSizeAsync (AddressablesLabels.Client)
                    .ToUniTask (_networkReachableReporterProgressFactory.Create ());
                _logger.Information ("GetDownloadSize Done {DownloadSize}", getDownloadSize);
                if (getDownloadSize > 0)
                {
                    _logger.Information ("DownloadDependencies...");
                    var handle = Addressables.DownloadDependenciesAsync (AddressablesLabels.Client);
                    _splashScreenPresenter.Display (
                        await _bundlesUpdateMessage.GetLocalizedStringAsync (),
                        handle.AsProgressReactiveProperty ());

                    await handle.ToUniTask (_networkReachableReporterProgressFactory.Create ());
                    Addressables.Release (handle);
                    _logger.Information ("DownloadDependencies Done");

                    LocalizationSettings.Instance.ResetState ();
                }
            } catch (OperationException e)
            {
                /**
                 * Checking this way because sometimes InnerException is null and there is no chance
                 * to get to ResourceManagerException in "proper" way (through inner exceptions chain)
                 */
                var isConnectionError = e.ToString ()
                    .Contains (
                        "UnityWebRequest result : ConnectionError",
                        StringComparison.CurrentCultureIgnoreCase);

                if (isConnectionError)
                {
                    if (++_updateAddressablesAttempts >= _networkingConfig.BeforeUnreachableNotifyAttempts)
                        _messageBroker.Publish (NetworkReachabilitySignal.UnreachableSignal);

                    await UniTask.Delay (TimeSpan.FromSeconds (_networkingConfig.RequestRetryRate));
                    await UpdateBundles ();
                } else
                {
                    throw;
                }
            }
        }

        private void OnRestart (RestartSignal _)
        {
            async UniTask RestartAsync ()
            {
                var restartMessage = await _restartMessage.GetLocalizedStringAsync ();
                _splashScreenPresenter.Display (restartMessage);

                foreach (var restartListener in _restartListeners)
                {
                    restartListener.OnRestart ();
                }

                _mutableWalletBridgeProvider.WalletBridgeUrl = await _updateHandlingService.PerformVersionCheck ();
                _updateAddressablesAttempts = 0;

                await UpdateBundles ();
                await _scenesLoadingService.RemoveAllScenesExceptBootstrap (restartMessage);
                await _scenesLoadingService.AppendScene (Scenes.HomeScreen, false, restartMessage, false);
            }


            RestartAsync ().Forget ();
        }
    }
}