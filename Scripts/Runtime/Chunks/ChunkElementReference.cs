using UnityEngine;

namespace gta_mirror.Chunks
{
    [System.Serializable]
    public class ChunkElementReference
    {
        public string chunkElementId;
        public Vector3 position;
        public Quaternion rotation;
    }
}