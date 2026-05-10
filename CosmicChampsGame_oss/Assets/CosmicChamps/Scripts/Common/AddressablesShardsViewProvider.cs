using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CosmicChamps.Common
{
    public class AddressablesShardsViewProvider : IShardsViewProvider, IRestartListener
    {
        private readonly Dictionary<string, AsyncOperationHandle<Sprite>> _spriteHandles = new();

        public async UniTask<Sprite> GetShardsIcon (string id)
        {
            var addressableId = $"shards[{id}]";
            if (!_spriteHandles.TryGetValue (addressableId, out var handle))
            {
                handle = Addressables.LoadAssetAsync<Sprite> (addressableId);
                _spriteHandles.Add (addressableId, handle);
            }

            if (!handle.IsDone)
                await handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
                throw new InvalidOperationException ($"Cannot find shards icon for {id}");

            return handle.Result;
        }

        public async UniTask SetDefaultTMPSprites ()
        {
            //When 'Use Asset Database' for Addressables, changes from runtime persisted. To prevent it, exclude setting default sprite asset in the editor
            #if !UNITY_EDITOR
            TMP_Settings.defaultSpriteAsset = await Addressables.LoadAssetAsync<TMP_SpriteAsset> ("shards.TMPSprites");
            #endif
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