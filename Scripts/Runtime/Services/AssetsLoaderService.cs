using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
                return default;

            string key = assetReference.RuntimeKey.ToString();

            if (_loadedAssets.TryGetValue(key, out var existingHandle) && existingHandle.IsValid())
            {
                return existingHandle.Result.GetComponent<T>();
            }

            var handle = Addressables.LoadAssetAsync<GameObject>(assetReference);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            {
                _loadedAssets[key] = handle;
                return handle.Result.GetComponent<T>();
            }

            return default;
        }
        
        public T LoadAssetSync<T>(AssetReferenceGameObject assetReference) where T : class
        {
            if (!assetReference.RuntimeKeyIsValid())
                return default;

            string key = assetReference.RuntimeKey.ToString();

            if (_loadedAssets.TryGetValue(key, out var existingHandle) && existingHandle.IsValid())
            {
                return existingHandle.Result.GetComponent<T>();
            }

            var handle = Addressables.LoadAssetAsync<GameObject>(assetReference);
            handle.WaitForCompletion();

            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            {
                _loadedAssets[key] = handle;
                return handle.Result.GetComponent<T>();
            }

            return default;
        }

        public void ReleaseAsset(AssetReferenceGameObject assetReference)
        {
            if (!assetReference.RuntimeKeyIsValid())
                return;

            string key = assetReference.RuntimeKey.ToString();
            if (_loadedAssets.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                _loadedAssets.Remove(key);
            }
        }

        public void ReleaseAllAssets()
        {
            foreach (var handle in _loadedAssets.Values)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
            _loadedAssets.Clear();
        }
    }
}