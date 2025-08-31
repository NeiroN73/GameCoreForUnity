using System.Collections.Generic;
using UnityEngine;

namespace gta_mirror.Chunks
{
    [System.Serializable]
    public class ChunkData : ScriptableObject
    {
        public string chunkName;
        public List<ChunkElementReference> chunkElementReferences;
    }
}