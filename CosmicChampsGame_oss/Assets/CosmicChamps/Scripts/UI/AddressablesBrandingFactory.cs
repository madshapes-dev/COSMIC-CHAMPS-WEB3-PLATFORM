using System.Collections.Generic;
using System.Threading.Tasks;
using CosmicChamps.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace CosmicChamps.UI
{
    public class AddressablesBrandingFactory : IBrandingFactory, IRestartListener
    {
        private const string BrandingPrefix = "branding";
        private const string CountdownPrefix = "countdown";
        private const string WinPrefix = "win";
        private const string LossPrefix = "loss";

        private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _cache = new();

        private void ClearCache ()
        {
            foreach (var handle in _cache.Values)
            {
                Addressables.Release (handle);
            }

            _cache.Clear ();
        }

        private async Task<GameObject> GetBranding (string key, Transform parent)
        {
            AsyncOperationHandle<IList<IResourceLocation>> resourceLocationHandle = default;
            GameObject instance = null;

            do
            {
                if (_cache.TryGetValue (key, out var prefabHandle))
                {
                    instance = Object.Instantiate (prefabHandle.Result, parent);
                    break;
                }

                resourceLocationHandle = Addressables.LoadResourceLocationsAsync (key);
                var resourceLocation = await resourceLocationHandle;
                if (resourceLocationHandle.Status != AsyncOperationStatus.Succeeded || resourceLocation.Count == 0)
                    break;

                prefabHandle = Addressables.LoadAssetAsync<GameObject> (key);
                await prefabHandle;

                if (prefabHandle.Status != AsyncOperationStatus.Succeeded)
                    break;

                instance = Object.Instantiate (prefabHandle.Result, parent);
                _cache.Add (key, prefabHandle);
            } while (false);

            if (resourceLocationHandle.IsValid ())
                Addressables.Release (resourceLocationHandle);

            return instance;
        }

        public Task<GameObject> GetCountdown (string opponentId, Transform parent = null) =>
            GetBranding ($"{BrandingPrefix}/{CountdownPrefix}/{opponentId}", parent);

        public Task<GameObject> GetWin (string opponentId, Transform parent = null) =>
            GetBranding ($"{BrandingPrefix}/{WinPrefix}/{opponentId}", parent);

        public Task<GameObject> GetLoss (string opponentId, Transform parent = null) =>
            GetBranding ($"{BrandingPrefix}/{LossPrefix}/{opponentId}", parent);

        public void OnRestart ()
        {
            ClearCache ();
        }
    }
}