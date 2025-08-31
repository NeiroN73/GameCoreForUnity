using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using gta_mirror.Scripts.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.Serialization;

namespace gta_mirror.Chunks
{
    public class ChunksLoader : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [SerializeField, LabelText("Load On Start")]
        private bool _loadOnStart = true;

        [BoxGroup("Settings")]
        [SerializeField, LabelText("Chunk Name Prefix")]
        private string _chunkPrefix = "Chunk";

        [BoxGroup("Settings")]
        [SerializeField, LabelText("Chunk Elements Label")]
        private string _chunkElementsLabel = "ChunkElement";

        [BoxGroup("Loading Priority")]
        [SerializeField, LabelText("Priority Radius")]
        private float _priorityRadius = 5000f;

        [BoxGroup("Loading Priority")]
        [SerializeField, LabelText("Load Priority Chunks First")]
        private bool _loadPriorityFirst = true;

        [BoxGroup("Runtime Info")]
        [ShowInInspector, ReadOnly, ProgressBar(0, 100)]
        private float _loadingProgress;

        [BoxGroup("Runtime Info")]
        [ShowInInspector, ReadOnly]
        private List<string> _loadedChunks = new List<string>();

        [BoxGroup("Runtime Info")]
        [ShowInInspector, ReadOnly]
        private List<string> _availableChunks = new List<string>();

        [BoxGroup("Runtime Info")]
        [ShowInInspector, ReadOnly]
        private List<ChunkDistanceInfo> _chunkDistances = new List<ChunkDistanceInfo>();

        private List<AsyncOperationHandle> _handles = new List<AsyncOperationHandle>();
        private Transform _root;
        private Dictionary<string, GameObject> _chunkElementsPrefabsCache = new Dictionary<string, GameObject>();
        private Dictionary<string, Vector3> _chunkCenterPositions = new Dictionary<string, Vector3>();
        private Vector3 playerPosition => PlayerPositionStatic.PlayerPosition;

        private void Start()
        {
            if (_loadOnStart)
            {
                DiscoverAndLoadChunks();
            }
        }

        public async void DiscoverAndLoadChunks()
        {
            await FindAvailableChunks();
            if (_availableChunks.Count > 0)
            {
                await CalculateChunkDistances();
                await LoadChunksByPriority();
            }
            else
            {
                Debug.LogWarning("No chunks found in Addressables. Build Addressables first.");
            }
        }

        private async UniTask FindAvailableChunks()
        {
            var chunkLocations = new List<IResourceLocation>();
    
            try
            {
                var locationsHandle = Addressables.LoadResourceLocationsAsync(_chunkPrefix, typeof(ChunkData));
                await locationsHandle.ToUniTask();

                if (locationsHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    chunkLocations.AddRange(locationsHandle.Result);
                }
                else
                {
                    Debug.LogWarning("Failed to load chunk locations by label");
                }
                Addressables.Release(locationsHandle);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load chunk locations: {e.Message}");
            }

            _availableChunks.Clear();
            foreach (var location in chunkLocations)
            {
                _availableChunks.Add(location.PrimaryKey);
            }
        }

        private async UniTask CalculateChunkDistances()
        {
            _chunkDistances.Clear();
            _chunkCenterPositions.Clear();

            foreach (var chunkName in _availableChunks)
            {
                try
                {
                    var handle = Addressables.LoadAssetAsync<ChunkData>(chunkName);
                    await handle.ToUniTask();

                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        var chunkData = handle.Result;
                    
                        Vector3 center = Vector3.zero;
                        if (chunkData.chunkElementReferences.Count > 0)
                        {
                            foreach (var elementRef in chunkData.chunkElementReferences)
                            {
                                center += elementRef.position;
                            }
                            center /= chunkData.chunkElementReferences.Count;
                        }
                    
                        float distance = Vector3.Distance(playerPosition, center);
                        bool isPriority = distance <= _priorityRadius;

                        _chunkDistances.Add(new ChunkDistanceInfo
                        {
                            chunkName = chunkName,
                            distance = distance,
                            isPriority = isPriority
                        });

                        _chunkCenterPositions[chunkName] = center;
                    }
                    Addressables.Release(handle);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error calculating distance for chunk {chunkName}: {e.Message}");
                }
            }

            _chunkDistances = _chunkDistances.OrderBy(c => c.distance).ToList();
        }

        private async UniTask LoadChunksByPriority()
        {
            if (_root == null)
            {
                _root = new GameObject("LoadedChunks").transform;
                _root.SetParent(transform);
            }

            await PreloadAllChunkElements();

            var priorityChunks = _chunkDistances.Where(c => c.isPriority).ToList();
            var nonPriorityChunks = _chunkDistances.Where(c => !c.isPriority).ToList();

            if (_loadPriorityFirst)
            {
                await LoadChunkList(priorityChunks, "Priority");
                await LoadChunkList(nonPriorityChunks, "Non-Priority");
            }
            else
            {
                await LoadChunkList(_chunkDistances, "All");
            }
        }

        private async UniTask LoadChunkList(List<ChunkDistanceInfo> chunksToLoad, string category)
        {
            Debug.Log($"Start loading {chunksToLoad.Count} {category} chunks");

            for (int i = 0; i < chunksToLoad.Count; i++)
            {
                var chunkInfo = chunksToLoad[i];
                await LoadChunk(chunkInfo.chunkName);
            
                _loadingProgress = (float)(_loadedChunks.Count) / _chunkDistances.Count * 100f;
            }
        
            Debug.Log($"Finish loading {chunksToLoad.Count} {category} chunks");
        }

        private async UniTask PreloadAllChunkElements()
        {
            try
            {
                var locationsHandle = Addressables.LoadResourceLocationsAsync("All_ChunkElements", typeof(GameObject));
                await locationsHandle.ToUniTask();

                if (locationsHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    foreach (var location in locationsHandle.Result)
                    {
                        var handle = Addressables.LoadAssetAsync<GameObject>(location);
                        await handle.ToUniTask();

                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            _chunkElementsPrefabsCache[location.PrimaryKey] = handle.Result;
                            _handles.Add(handle);
                        }
                    }
                }
                Addressables.Release(locationsHandle);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to preload chunk elements: {e.Message}");
            }
        }

        private async UniTask LoadChunk(string chunkName)
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<ChunkData>(chunkName);
                await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var chunkData = handle.Result;
                    InstantiateChunk(chunkData);
                
                    _loadedChunks.Add(chunkName);
                    _handles.Add(handle);
                }
                else
                {
                    Debug.LogError($"Failed to load chunk: {chunkName}");
                    Addressables.Release(handle);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading chunk {chunkName}: {e.Message}");
            }
        }

        private void InstantiateChunk(ChunkData chunkData)
        {
            var chunkRoot = new GameObject(chunkData.chunkName);
            chunkRoot.transform.SetParent(_root);

            foreach (var chunkElementRef in chunkData.chunkElementReferences)
            {
                if (_chunkElementsPrefabsCache.TryGetValue(chunkElementRef.chunkElementId, out var chunkElementPrefab))
                {
                    var chunkElementInstance = Instantiate(chunkElementPrefab, 
                        chunkElementRef.position, chunkElementRef.rotation, chunkRoot.transform);
                    chunkElementInstance.name = chunkElementRef.chunkElementId;
                
                    var chunkElementComponent = chunkElementInstance.GetComponent<ChunkElement>();
                    if (chunkElementComponent != null)
                    {
                        chunkElementComponent.CurrentChunk = chunkData.chunkName;
                        chunkElementComponent.AddressableKey = chunkElementRef.chunkElementId;
                    }
                }
                else
                {
                    LoadChunkElementAsync(chunkElementRef, chunkRoot.transform);
                }
            }
        }

        private async void LoadChunkElementAsync(ChunkElementReference chunkElementRef, Transform parent)
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(chunkElementRef.chunkElementId);
                await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var chunkElementInstance = Instantiate(handle.Result, 
                        chunkElementRef.position, chunkElementRef.rotation, parent);
                    chunkElementInstance.name = chunkElementRef.chunkElementId;
                    _handles.Add(handle);
                }
                else
                {
                    Addressables.Release(handle);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading chunk element {chunkElementRef.chunkElementId}: {e.Message}");
            }
        }

        public void UnloadAll()
        {
            foreach (var handle in _handles)
            {
                if (handle.IsValid()) 
                {
                    Addressables.Release(handle);
                }
            }
            _handles.Clear();
            _loadedChunks.Clear();
            _chunkElementsPrefabsCache.Clear();
            _chunkCenterPositions.Clear();
            _chunkDistances.Clear();

            if (_root != null)
            {
                Destroy(_root.gameObject);
            }

            _loadingProgress = 0;
        }

        private void OnDestroy()
        {
            UnloadAll();
        }
    }
}