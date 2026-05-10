using System.Linq;
using CosmicChamps.Settings;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using Serilog.Core;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Services
{
    public class AdService : IInitializable
    {
        private readonly ILogger _logger;
        private readonly AppProfile _appProfile;
        private InterstitialAd _ad;

        public AdService (ILogger logger, AppProfile appProfile)
        {
            _logger = logger;
            _appProfile = appProfile;
        }

        private void Log (string messageTemplate)
        {
            async UniTaskVoid InternalLog ()
            {
                await UniTask.SwitchToMainThread ();
                _logger.Information (messageTemplate);
            }

            InternalLog ().Forget ();
        }

        [MessageTemplateFormatMethod ("messageTemplate")]
        private void Log<T> (string messageTemplate, T propertyValue)
        {
            async UniTaskVoid InternalLog ()
            {
                await UniTask.SwitchToMainThread ();
                _logger.Information (messageTemplate, propertyValue);
            }

            InternalLog ().Forget ();
        }

        #if UNITY_IOS || UNITY_ANDROID
        public void Initialize ()
        {
            Log ("Ad mob initializing...");
            MobileAds.Initialize (
                status =>
                {
                    var adapterStatus = string
                        .Join (
                            "; ",
                            status
                                .getAdapterStatusMap ()
                                .Select (x => $"{x.Key}: {x.Value.InitializationState}, '{x.Value.Description}'"));

                    Log ("Ad mob initialized {@Status}", adapterStatus);
                });
        }

        public UniTask<bool> Load ()
        {
            if (_ad != null)
            {
                _ad.Destroy ();
                _ad = null;
            }

            Log ("Loading the rewarded interstitial ad");

            var adRequest = new AdRequest ();
            var completionSource = new UniTaskCompletionSource<bool> ();
            InterstitialAd.Load (
                _appProfile.AdMobUnitId,
                adRequest,
                (ad, error) =>
                {
                    async UniTaskVoid ProcessResult ()
                    {
                        var failed = error != null || ad == null;
                        if (failed)
                        {
                            Log ("Rewarded interstitial ad failed to load an ad with error: {Error}", error);
                        } else
                        {
                            Log (
                                "Rewarded interstitial ad loaded with response: {ResponseInfo}",
                                ad.GetResponseInfo ());
                            _ad = ad;
                        }

                        await UniTask.SwitchToMainThread ();
                        completionSource.TrySetResult (!failed);
                    }

                    ProcessResult ().Forget ();
                });

            return completionSource.Task;
        }

        public UniTask Show ()
        {
            if (_ad == null)
            {
                Log ("Interstitial Ad full screen content closed");
                return UniTask.CompletedTask;
            }

            if (!_ad.CanShowAd ())
            {
                Log ("Cannot show the ad");
                return UniTask.CompletedTask;
            }

            var completionSource = new UniTaskCompletionSource ();

            _ad.OnAdFullScreenContentClosed += AdOnOnAdFullScreenContentClosed;
            _ad.Show ();

            return completionSource.Task;

            void AdOnOnAdFullScreenContentClosed ()
            {
                async UniTaskVoid ProcessResult ()
                {
                    Log ("Interstitial Ad full screen content closed");

                    await UniTask.SwitchToMainThread ();
                    completionSource.TrySetResult ();
                }

                ProcessResult ().Forget ();
            }
        }
        #else
        public void Initialize ()
        {
        }

        public UniTask<bool> Load () => UniTask.FromResult (false);
        public UniTask Show () => UniTask.CompletedTask;
        #endif
    }
}