using UnityEngine;

namespace World
{
    public class ChunkFactory
    {
        public static ChunkData CreateChunk(int x, int z, Vector3 position, int size, int maxY)
        {
            ChunkData chunk = new ChunkData();

            chunk.x = x;
            chunk.z = z;
            chunk.origin = position;
            chunk.MaxX = size;
            chunk.MaxY = maxY;
            chunk.MaxZ = size;
            chunk.blocks = new BlockData[size, maxY, size];
            chunk.meshData = new ChunkMeshData();
            chunk.meshChanged = false;

            return chunk;
        }
    }
}
