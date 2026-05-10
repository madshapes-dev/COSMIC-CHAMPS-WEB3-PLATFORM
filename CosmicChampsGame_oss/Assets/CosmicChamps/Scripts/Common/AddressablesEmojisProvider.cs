using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CosmicChamps.Common
{
    public class AddressablesEmojisProvider : IEmojisProvider, IRestartListener
    {
        private readonly Dictionary<string, AsyncOperationHandle<Sprite>> _spriteHandles = new();

        public async UniTask<Sprite> GetEmoji (string id)
        {
            var addressableId = $"emojis/{id}";
            if (!_spriteHandles.TryGetValue (addressableId, out var handle))
            {
                handle = Addressables.LoadAssetAsync<Sprite> (addressableId);
                _spriteHandles.Add (addressableId, handle);
            }

            if (!handle.IsDone)
                await handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
                throw new InvalidOperationException ($"Cannot find emoji {id}");

            return handle.Result;
        }

        public void OnRestart ()
        {
            foreach (var spriteHandle in _spriteHandles.Values)
            {
                Addressables.Release (spriteHandle);
            }

            _spriteHandles.Clear ();
        }
    }
}