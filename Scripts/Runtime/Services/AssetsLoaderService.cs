using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameCore.Services
{
    public class AssetsLoaderService : Service
    {
        private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _loadedAssets = new();
        
        public async UniTask<T> LoadAssetAsync<T>(AssetReferenceGameObject assetReference) where T : class
        {
            if (!assetReference.RuntimeKeyIsValid())
            {
                return default;
            }

            string key = assetReference.RuntimeKey.ToString();

            if (TryGetCachedAsset<T>(key, out var cachedAsset))
            {
                return cachedAsset;
            }

            var handle = Addressables.LoadAssetAsync<GameObject>(assetReference);
            await handle.Task;

            return ProcessLoadedHandle<T>(key, handle);
        }
        
        public T LoadAssetSync<T>(AssetReferenceGameObject assetReference) where T : class
        {
            if (!assetReference.RuntimeKeyIsValid())
            {
                return default;
            }

            string key = assetReference.RuntimeKey.ToString();

            if (TryGetCachedAsset<T>(key, out var cachedAsset))
            {
                return cachedAsset;
            }
            
            var handle = Addressables.LoadAssetAsync<GameObject>(assetReference);
            handle.WaitForCompletion();

            return ProcessLoadedHandle<T>(key, handle);
        }

        private bool TryGetCachedAsset<T>(string key, out T asset) where T : class
        {
            if (_loadedAssets.TryGetValue(key, out var existingHandle) && existingHandle.IsValid())
            {
                asset = existingHandle.Result.GetComponent<T>();
                return asset != null;
            }

            asset = default;
            return false;
        }

        private T ProcessLoadedHandle<T>(string key, AsyncOperationHandle<GameObject> handle) where T : class
        {
            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            {
                _loadedAssets[key] = handle;
                return handle.Result.GetComponent<T>();
            }

            Addressables.Release(handle);
            return default;
        }

        public void ReleaseAsset(AssetReferenceGameObject assetReference)
        {
            if (!assetReference.RuntimeKeyIsValid())
                return;

            string key = assetReference.RuntimeKey.ToString();
            if (_loadedAssets.Remove(key, out var handle) && handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }

        public void ReleaseAllAssets()
        {
            foreach (var handle in _loadedAssets.Values)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            _loadedAssets.Clear();
        }
    }
}