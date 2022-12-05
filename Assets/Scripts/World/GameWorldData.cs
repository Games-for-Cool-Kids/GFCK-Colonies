using System;

namespace World
{
    [Serializable]
    public partial class GameWorldChunkData
    {
        public int chunkSize = 32;
        public int worldChunkWidth; // Number of chunks in x/z direction.
        public int blockHeight;

        public Chunk[,] chunks;
    }
}
