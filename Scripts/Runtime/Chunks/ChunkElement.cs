using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace gta_mirror.Chunks
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class ChunkElement : MonoBehaviour
    {
        [SerializeField] [ReadOnly] private string _chunkElementId;
        [SerializeField] [ReadOnly] private float _sizeMb;
        public bool includeInChunking = true;
        [ReadOnly] public string CurrentChunk { get; set; }
        [ReadOnly] public string AddressableKey { get; set; }

        public string GetChunkElementId()
        {
            if (_chunkElementId == Guid.Empty.ToString())
                _chunkElementId = GenerateChunkElementId();
        
            return _chunkElementId;
        }
    
#if UNITY_EDITOR
        private void Reset()
        {
            _chunkElementId = GenerateChunkElementId();
        }

        private void OnValidate()
        {
            if (Application.isEditor &&!Application.isPlaying)
            {
                if (string.IsNullOrEmpty(_chunkElementId))
                {
                    _chunkElementId = GenerateChunkElementId();
                }
            }
        }

        private string GenerateChunkElementId()
        {
            return name + Guid.NewGuid();
        }
    
        [Button(ButtonSizes.Medium)]
        private void ProcessElementSize()
        {
            var size = AddressablesSizeCalculator.GetAssetSizeOptimized(_chunkElementId);

            if (size > 0)
            {
                _sizeMb = AddressablesSizeCalculator.BytesToMB(size);
            }
        }
#endif
    }
}