using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using gta_mirror.Chunks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace gta_mirror.Editor
{
    public class ChunksEditorWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Chunks Editor")]
        private static void OpenWindow()
        {
            var window = GetWindow<ChunksEditorWindow>();
            window.titleContent = new GUIContent("Chunks Editor");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }

        [BoxGroup("Settings", Order = 0)]
        [SerializeField, LabelText("Chunk Size (MB)")]
        [Range(50, 500)]
        private int chunkSizeMB = 200;

        [BoxGroup("Settings")]
        [SerializeField, LabelText("Chunk Name Prefix")]
        private string chunkPrefix = "Chunk";

        [BoxGroup("Settings")]
        [SerializeField, LabelText("Chunks Folder")]
        [FolderPath]
        private string chunksFolder = "Assets/AddressableChunks";

        [BoxGroup("Chunk Elements", Order = 1)]
        [ShowInInspector, ReadOnly]
        [ListDrawerSettings(ShowPaging = true, NumberOfItemsPerPage = 10, Expanded = true)]
        private List<ChunkElementInfo> allChunkElements = new List<ChunkElementInfo>();

        [BoxGroup("Chunks", Order = 2)]
        [ListDrawerSettings(ShowPaging = true, NumberOfItemsPerPage = 5, Expanded = true)]
        [SerializeField, ReadOnly]
        private List<ChunkData> chunks = new List<ChunkData>();

        [BoxGroup("Preview", Order = 3)]
        [ShowInInspector, ReadOnly]
        [TableList(AlwaysExpanded = true)]
        private List<ChunkPreviewInfo> chunkPreviews = new List<ChunkPreviewInfo>();

        [BoxGroup("Statistics", Order = 4)]
        [ShowInInspector, ReadOnly]
        private string totalChunkElementsInfo;

        [BoxGroup("Statistics", Order = 4)]
        [ShowInInspector, ReadOnly]
        private string totalSizeInfo;

        private bool isScanningSizes = false;
        private int scannedElementsCount = 0;

        [BoxGroup("Actions", Order = 5)]
        [Button(ButtonSizes.Large), GUIColor(0.2f, 0.6f, 1f)]
        [PropertyOrder(0)]
        public void ScanSceneForChunkElements()
        {
            allChunkElements.Clear();
            chunks.Clear();
            chunkPreviews.Clear();

            var chunkElements = FindObjectsOfType<ChunkElement>()
                .Where(b => b.includeInChunking && b.gameObject.scene == SceneManager.GetActiveScene())
                .ToList();

            if (chunkElements.Count == 0)
            {
                return;
            }

            foreach (var chunkElement in chunkElements)
            {
                allChunkElements.Add(new ChunkElementInfo
                {
                    name = chunkElement.name,
                    sizeMB = 0f,
                    gameObject = chunkElement.gameObject,
                    addressableKey = chunkElement.GetChunkElementId()
                });
            }
        }
        
        [BoxGroup("Actions")]
        [Button(ButtonSizes.Large), GUIColor(0.3f, 0.7f, 0.3f)]
        [EnableIf("@allChunkElements.Count > 0 && !isScanningSizes")]
        [PropertyOrder(1)]
        public async void ProcessAllChunkOperations()
        {
            if (isScanningSizes) return;
            isScanningSizes = true;

            try
            {
                // 1. Сохранение всех элементов в Addressables
                if (!EnsureChunksFolderExists()) return;

                var settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings == null)
                {
                    return;
                }

                EditorUtility.DisplayProgressBar("Processing Chunks", "Saving elements to Addressables...", 0.1f);

                var chunkElementsGroup = settings.FindGroup("All_ChunkElements") ?? 
                                         settings.CreateGroup("All_ChunkElements", false, false, false, null);

                EnsureFolderExists($"{chunksFolder}/ChunkElements");

                for (int i = 0; i < allChunkElements.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("Processing Chunks", 
                        $"Saving element {i + 1}/{allChunkElements.Count}", 0.1f + (0.2f * i / allChunkElements.Count));

                    var elementInfo = allChunkElements[i];
                    var chunkElement = elementInfo.gameObject.GetComponent<ChunkElement>();
                    
                    if (chunkElement != null)
                    {
                        SaveChunkElementToAddressables(chunkElement, chunkElementsGroup, settings);
                    }

                    Repaint();
                }

                // 2. Сканирование реальных размеров
                EditorUtility.DisplayProgressBar("Processing Chunks", "Scanning sizes...", 0.3f);
                
                scannedElementsCount = 0;
                var tasks = new List<UniTask>();
                var elementInfos = allChunkElements.ToArray();

                for (int i = 0; i < elementInfos.Length; i++)
                {
                    var elementInfo = elementInfos[i];
                    tasks.Add(ProcessElementSize(elementInfo, i, elementInfos.Length));
                }

                await UniTask.WhenAll(tasks);

                // 3. Создание чанков из элементов
                EditorUtility.DisplayProgressBar("Processing Chunks", "Creating chunks from elements...", 0.6f);
                
                chunks.Clear();
                chunkPreviews.Clear();

                var elementsWithSize = allChunkElements.Select(e => new {
                    Element = e,
                    SizeMB = e.sizeMB
                }).ToList();

                var sortedElements = elementsWithSize
                    .OrderByDescending(e => e.SizeMB)
                    .Select(e => e.Element)
                    .ToList();

                CreateChunksFromSortedElements(sortedElements);
                UpdatePreviews();
                UpdateStatistics();

                // 4. Создание Addressable чанков
                if (chunks.Count > 0)
                {
                    EditorUtility.DisplayProgressBar("Processing Chunks", "Creating Addressable chunks...", 0.8f);

                    if (!EnsureChunksFolderExists()) return;

                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

                    for (int i = 0; i < chunks.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Processing Chunks", 
                            $"Creating chunk {i + 1}/{chunks.Count}", 0.8f + (0.2f * i / chunks.Count));

                        var chunk = chunks[i];
                        ProcessChunk(settings, chunk);

                        Repaint();
                    }
                }
                
                EditorUtility.DisplayProgressBar("Processing schemas", "Add Addressable Schemas To Group...", 0.99f);
                
                AddContentPackingLoadingSchema(chunkElementsGroup);
                
                var chunksGroup = settings.FindGroup("All_Chunks") ?? 
                                         settings.CreateGroup("All_Chunks", false, false, false, null);

                AddContentPackingLoadingSchema(chunksGroup);

                EditorUtility.ClearProgressBar();
                Debug.Log("All chunk operations completed successfully!");
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError($"Error processing chunks: {e}");
            }
            finally
            {
                isScanningSizes = false;
            }
        }
        
        [BoxGroup("Actions")]
        [Button(ButtonSizes.Large), GUIColor(1f, 0.8f, 0.2f)]
        [PropertyOrder(5)]
        public void SetupRuntimeLoader()
        {
            var loader = FindObjectOfType<ChunksLoader>();
            if (loader == null)
            {
                var go = new GameObject("ChunksLoader");
                loader = go.AddComponent<ChunksLoader>();
                EditorUtility.SetDirty(loader);
            }
        }

        [BoxGroup("Actions")]
        [Button(ButtonSizes.Medium), GUIColor(0.8f, 0.4f, 0.4f)]
        [PropertyOrder(6)]
        public void CleanupChunks()
        {
            if (EditorUtility.DisplayDialog("Cleanup Chunks", 
                    "This will delete all chunk prefabs and Addressables groups. Continue?", "Yes", "No"))
            {
                if (AssetDatabase.IsValidFolder(chunksFolder))
                {
                    AssetDatabase.DeleteAsset(chunksFolder);
                    AssetDatabase.Refresh();
                }

                RemoveAddressablesGroups();

                chunks.Clear();
                chunkPreviews.Clear();
            }
        }
        
        private void AddContentPackingLoadingSchema(AddressableAssetGroup group)
        {
            if (group == null) return;

            var schema = group.GetSchema<BundledAssetGroupSchema>();
            if (schema == null)
            {
                group.AddSchema<BundledAssetGroupSchema>();
            }
        }
        
       private async UniTask ProcessElementSize(ChunkElementInfo elementInfo, int index, int totalCount)
       {
           try
           {
               string cleanKey = elementInfo.addressableKey.Trim();
               cleanKey = cleanKey.Replace(" (1)", "");
       
               long size = AddressablesSizeCalculator.GetAssetSizeOptimized(cleanKey);
       
               if (size > 0)
               {
                   elementInfo.sizeMB = AddressablesSizeCalculator.BytesToMB(size);
                   Debug.Log($"Final size for {cleanKey}: {AddressablesSizeCalculator.FormatBytes(size)}");
               }
           }
           catch (System.Exception e)
           {
               Debug.LogError($"Failed to get size for {elementInfo.addressableKey}: {e.Message}");
           }
       }

        private void CreateChunksFromSortedElements(List<ChunkElementInfo> sortedElements)
        {
            var currentChunk = new ChunkData { chunkName = $"{chunkPrefix}{chunks.Count}" };
            float currentSizeMB = 0;
            float maxSizeMB = chunkSizeMB;

            foreach (var elementInfo in sortedElements)
            {
                var chunkElement = elementInfo.gameObject.GetComponent<ChunkElement>();
                if (chunkElement == null) continue;

                float elementSize = elementInfo.sizeMB;

                if (currentSizeMB + elementSize > maxSizeMB && currentChunk.chunkElements.Count > 0)
                {
                    chunks.Add(currentChunk);
                    currentChunk = new ChunkData { chunkName = $"{chunkPrefix}{chunks.Count}" };
                    currentSizeMB = 0;
                }

                currentChunk.chunkElements.Add(chunkElement);
                currentChunk.chunkSize += (long)(elementSize * 1024 * 1024);
                currentSizeMB += elementSize;
                
                chunkElement.CurrentChunk = currentChunk.chunkName;
            }

            if (currentChunk.chunkElements.Count > 0)
            {
                chunks.Add(currentChunk);
            }
        }

        private void SaveChunkElementToAddressables(ChunkElement chunkElement, AddressableAssetGroup group, AddressableAssetSettings settings)
        {
            var chunkElementId = chunkElement.GetChunkElementId();
            var prefabPath = $"{chunksFolder}/ChunkElements/{chunkElementId}.prefab";

            GameObject chunkElementPrefab;

            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null)
            {
                chunkElementPrefab = PrefabUtility.SaveAsPrefabAsset(chunkElement.gameObject, prefabPath);
            }
            else
            {
                chunkElementPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            }

            if (chunkElementPrefab != null)
            {
                var chunkElementPath = AssetDatabase.GetAssetPath(chunkElementPrefab);
                var chunkElementGuid = AssetDatabase.AssetPathToGUID(chunkElementPath);
                
                var chunkElementEntry = settings.CreateOrMoveEntry(chunkElementGuid, group);
                chunkElementEntry.address = chunkElementId;
                
                chunkElementEntry.labels.Add("ChunkElement");
                chunkElementEntry.labels.Add(chunkPrefix);
                
                chunkElement.AddressableKey = chunkElementId;
            }
        }

        private void ProcessChunk(AddressableAssetSettings settings, ChunkData chunk)
        {
            var chunksGroup = settings.FindGroup("All_Chunks") ?? 
                             settings.CreateGroup("All_Chunks", false, false, false, null);
            
            var chunkElementsGroup = settings.FindGroup("All_ChunkElements") ?? 
                                   settings.CreateGroup("All_ChunkElements", false, false, false, null);

            EnsureFolderExists($"{chunksFolder}/ChunkElements");
            EnsureFolderExists($"{chunksFolder}/Chunks");

            var chunkData = CreateInstance<Chunks.ChunkData>();
            chunkData.chunkName = chunk.chunkName;
            chunkData.chunkElementReferences = new List<ChunkElementReference>();

            foreach (var chunkElement in chunk.chunkElements)
            {
                var chunkElementId = chunkElement.GetChunkElementId();
                var prefabPath = $"{chunksFolder}/ChunkElements/{chunkElementId}.prefab";

                GameObject chunkElementPrefab = null;

                if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null)
                {
                    chunkElementPrefab = PrefabUtility.SaveAsPrefabAsset(chunkElement.gameObject, prefabPath);
                    
                    if (chunkElementPrefab != null)
                    {
                        var chunkElementPath = AssetDatabase.GetAssetPath(chunkElementPrefab);
                        var chunkElementGuid = AssetDatabase.AssetPathToGUID(chunkElementPath);
                        
                        var chunkElementEntry = settings.CreateOrMoveEntry(chunkElementGuid, chunkElementsGroup);
                        chunkElementEntry.address = chunkElementId;
                        
                        chunkElementEntry.labels.Add("ChunkElement");
                    }
                }
                else
                {
                    chunkElementPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                }

                if (chunkElementPrefab != null)
                {
                    chunkData.chunkElementReferences.Add(new ChunkElementReference
                    {
                        chunkElementId = chunkElementId,
                        position = chunkElement.transform.position,
                        rotation = chunkElement.transform.rotation
                    });
                    
                    chunkElement.CurrentChunk = chunk.chunkName;
                    chunkElement.AddressableKey = chunkElementId;
                }
            }

            var chunkDataPath = $"{chunksFolder}/Chunks/{chunk.chunkName}_Data.asset";
            AssetDatabase.CreateAsset(chunkData, chunkDataPath);

            var dataGuid = AssetDatabase.AssetPathToGUID(chunkDataPath);
            var dataEntry = settings.CreateOrMoveEntry(dataGuid, chunksGroup);
            dataEntry.address = chunk.chunkName;
            
            dataEntry.labels.Add(chunkPrefix);
            
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }
        
        private void EnsureFolderExists(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }
        }

        private void RemoveAddressablesGroups()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogWarning("Addressable Settings not found!");
                return;
            }

            var groupsToRemove = new List<AddressableAssetGroup>();
        
            foreach (var group in settings.groups)
            {
                if (group.Name == "All_Chunks" || group.Name == "All_ChunkElements")
                {
                    groupsToRemove.Add(group);
                }
            }

            foreach (var group in groupsToRemove)
            {
                settings.RemoveGroup(group);
                Debug.Log($"Removed Addressables group: {group.Name}");
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupRemoved, null, true, true);
            AssetDatabase.SaveAssets();
        }
        
        private void UpdatePreviews()
        {
            chunkPreviews = chunks.Select(chunk => new ChunkPreviewInfo
            {
                chunkName = chunk.chunkName,
                chunkElementCount = chunk.chunkElements.Count,
                sizeMB = chunk.chunkSize / (1024f * 1024f),
                chunkElements = chunk.chunkElements.Select(b => 
                {
                    var elementInfo = allChunkElements.FirstOrDefault(e => e.gameObject == b.gameObject);
                    float sizeMB;

                    if (elementInfo != null)
                    {
                        sizeMB = elementInfo.sizeMB;
                    }
                    else
                    {
                        sizeMB = CalculateChunkElementSize(b.gameObject) / (1024f * 1024f);
                    }

                    return new ChunkElementPreviewInfo
                    {
                        name = b.name,
                        sizeMB = sizeMB,
                    };
                }).ToList()
            }).ToList();
        }

        private void UpdateStatistics()
        {
            int totalChunkElements = chunks.Sum(c => c.chunkElements.Count);
            float totalSizeMB = chunks.Sum(c => c.chunkSize) / (1024f * 1024f);

            totalChunkElementsInfo = $"{totalChunkElements} chunkElements";
            totalSizeInfo = $"{totalSizeMB:F1} MB total";
        }

        private long CalculateChunkElementSize(GameObject chunkElement)
        {
            long size = 0;
            var renderers = chunkElement.GetComponentsInChildren<Renderer>();
            var filters = chunkElement.GetComponentsInChildren<MeshFilter>();

            foreach (var renderer in renderers)
                if (renderer.sharedMaterial != null) size += 2 * 1024 * 1024;

            foreach (var filter in filters)
                if (filter.sharedMesh != null) size += 1 * 1024 * 1024;

            return Math.Max(size, 1024 * 1024);
        }

        private bool EnsureChunksFolderExists()
        {
            if (!AssetDatabase.IsValidFolder(chunksFolder))
            {
                var parts = chunksFolder.Split('/');
                string currentPath = "";
                
                foreach (var part in parts)
                {
                    if (string.IsNullOrEmpty(part)) continue;
                    
                    currentPath = string.IsNullOrEmpty(currentPath) ? part : $"{currentPath}/{part}";
                    if (!AssetDatabase.IsValidFolder(currentPath))
                    {
                        AssetDatabase.CreateFolder(
                            currentPath.Contains("/") ? currentPath.Substring(0, currentPath.LastIndexOf('/')) : "Assets",
                            currentPath.Contains("/") ? currentPath.Substring(currentPath.LastIndexOf('/') + 1) : part
                        );
                    }
                }
            }
            return true;
        }

        [System.Serializable]
        public class ChunkData
        {
            [HorizontalGroup("Chunk")]
            [LabelText("Chunk Name")]
            public string chunkName;

            [HorizontalGroup("Chunk")]
            [LabelText("ChunkElements")]
            [ReadOnly]
            public int chunkElementCount => chunkElements.Count;

            [HorizontalGroup("Chunk")]
            [LabelText("Size MB")]
            [ReadOnly]
            public float sizeMB => chunkSize / (1024f * 1024f);

            [HideInInspector]
            public long chunkSize;

            [ListDrawerSettings(ShowPaging = true, NumberOfItemsPerPage = 10)]
            public List<ChunkElement> chunkElements = new List<ChunkElement>();
        }

        [System.Serializable]
        public class ChunkPreviewInfo
        {
            [TableColumnWidth(150)]
            public string chunkName;

            public int chunkElementCount;

            [TableColumnWidth(100)]
            [SuffixLabel("MB", true)]
            public float sizeMB;

            [TableColumnWidth(200)]
            [ListDrawerSettings(ShowPaging = true, NumberOfItemsPerPage = 3)]
            public List<ChunkElementPreviewInfo> chunkElements;
        }
        
        [System.Serializable]
        public class ChunkElementInfo
        {
            public string name;
            public float sizeMB;
            public GameObject gameObject;
            public string addressableKey;
        }

        [System.Serializable]
        public class ChunkElementPreviewInfo
        {
            public string name;
            public float sizeMB;
            public bool hasRealSize;
        }
    }
}
