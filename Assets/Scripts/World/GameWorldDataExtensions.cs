using UnityEngine;

namespace World
{
    public static class GameWorldDataExtensions
    {
        public static Chunk GetChunk(this GameWorldChunkData worldChunks, int x, int z)
        {
            if (x < 0 || x >= worldChunks.worldChunkWidth
            || z < 0 || z >= worldChunks.worldChunkWidth)
                return null;

            return worldChunks.chunks[x, z];
        }

        public static Chunk GetChunkAt(this GameWorldChunkData worldChunks, Vector3 worldPos)
        {
            Vector3 relativePos = worldPos + new Vector3(0.5f, 0.5f, 0.5f); // We need offset of half a block. Origin is middle of first block.

            int chunkX = Mathf.FloorToInt(relativePos.x / worldChunks.chunkSize);
            int chunkZ = Mathf.FloorToInt(relativePos.z / worldChunks.chunkSize);

            return worldChunks.GetChunk(chunkX, chunkZ);
        }

        /// <summary>Expects a position inside of the block.</summary>
        public static Block GetBlockAt(this GameWorldChunkData worldChunks, Vector3 worldPos)
        {
            var chunk = worldChunks.GetChunkAt(worldPos);
            if (chunk == null)
                return null;

            return chunk.GetBlockAt(worldPos);
        }
    }
}
