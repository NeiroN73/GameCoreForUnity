using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Content.Scripts.Services
{
    public class AssetsLoaderService : Service
    {
        private readonly Dictionary<string, AsyncOperationHandle> _loadedAssets = new();
        
        public async UniTask<T> LoadAssetAsync<T>(AssetReferenceT<T> assetReference) where T : Object
        {
            if (!assetReference.RuntimeKeyIsValid())
            {
                return null;
            }

            var handle = assetReference.LoadAssetAsync();
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            {
                string key = assetReference.RuntimeKey.ToString();
                _loadedAssets[key] = handle;
                return handle.Result;
            }

            return default;
        }
    }
}