using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Common
{
    public class AddressablesCardViewDataProvider : ICardViewDataProvider, IRestartListener
    {
        private readonly Dictionary<string, AsyncOperationHandle<Sprite>> _spriteHandles = new();
        private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _previewHandles = new();

        private ILogger _logger;

        public AddressablesCardViewDataProvider (ILogger logger)
        {
            _logger = logger;
        }

        private string GetCardAddressableId (string cardId, string unitSkinId)
        {
            var unitSkinIdChunks = unitSkinId.Split ('_');
            return unitSkinIdChunks.Length == 1 ? cardId : $"{cardId}_{unitSkinIdChunks[1]}";
        }

        public async UniTask<Sprite> GetCardSprite (string cardId, string unitSkinId)
        {
            var addressableId = $"cards/{GetCardAddressableId (cardId, unitSkinId)}";
            if (!_spriteHandles.TryGetValue (addressableId, out var handle))
            {
                handle = Addressables.LoadAssetAsync<Sprite> (addressableId);
                _spriteHandles.Add (addressableId, handle);
            }

            if (!handle.IsDone)
                await handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
                throw new InvalidOperationException ($"Cannot find card sprite for unit {cardId} skin {unitSkinId}");

            return handle.Result;
        }

        public async UniTask<GameObject> GetPreview (string cardId, string unitSkinId)
        {
            var addressableId = $"previews/{GetCardAddressableId (cardId, unitSkinId)}";
            if (!_previewHandles.TryGetValue (addressableId, out var handle))
            {
                handle = Addressables.LoadAssetAsync<GameObject> (addressableId);
                _previewHandles.Add (addressableId, handle);
            }

            if (!handle.IsDone)
                await handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
                throw new InvalidOperationException ($"Cannot find preview for unit {cardId} skin {unitSkinId}");

            return handle.Result;
        }

        public UniTask PrewarmPreviews ((string, string)[] ids) =>
            UniTask.WhenAll (ids.Select (x => GetPreview (x.Item1, x.Item2)));

        public void OnRestart ()
        {
            foreach (var spriteHandle in _spriteHandles.Values)
            {
                Addressables.Release (spriteHandle);
            }

            foreach (var previewHandle in _previewHandles.Values)
            {
                Addressables.Release (previewHandle);
            }

            _spriteHandles.Clear ();
            _previewHandles.Clear ();
        }
    }
}