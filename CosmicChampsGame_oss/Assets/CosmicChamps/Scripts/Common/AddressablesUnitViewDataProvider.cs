using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CosmicChamps.Common
{
    public class AddressablesUnitViewDataProvider : IUnitViewDataProvider, IRestartListener
    {
        private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _prefabHandles = new();

        public async UniTask<GameObject> GetPrefab (string id)
        {
            var addressableId = $"units/{id}";
            if (!_prefabHandles.TryGetValue (addressableId, out var handle))
            {
                handle = Addressables.LoadAssetAsync<GameObject> (addressableId);
                _prefabHandles.Add (addressableId, handle);
            }

            if (!handle.IsDone)
                await handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
                throw new InvalidOperationException ($"Cannot find prefab for unit {id}");

            return handle.Result;
        }

        public UniTask Prewarm (string[] ids) => UniTask.WhenAll (ids.Select (GetPrefab));

        public void OnRestart ()
        {
            foreach (var spriteHandle in _prefabHandles.Values)
            {
                Addressables.Release (spriteHandle);
            }

            _prefabHandles.Clear ();
        }
    }
}