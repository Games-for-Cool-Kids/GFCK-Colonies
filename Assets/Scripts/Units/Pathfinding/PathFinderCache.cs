using PathFinding;
using World;

namespace Pathfinding
{
    public class PathFinderCache
    {
        public NodeGrid[,] ChunkNodeGrids { get; private set; }

        private GameWorld _world;
        private int worldChunkWidth;

        public PathFinderCache(GameWorld world)
        {
            _world = world;

            GameManager.Instance.World.WorldGenerationDone += GenerateCache; // TODO Unregister
        }

        public void GenerateCache()
        {
            this.worldChunkWidth = _world.worldChunks.worldChunkWidth;

            GenerateWalkableChunkNodeGrids(_world.worldChunks.chunks);
        }

        private void GenerateWalkableChunkNodeGrids(Chunk[,] chunks)
        {
            ChunkNodeGrids = new NodeGrid[worldChunkWidth, worldChunkWidth];

            for (int x = 0; x < worldChunkWidth; x++)
            {
                for (int z = 0; z < worldChunkWidth; z++)
                {
                    var currentChunk = chunks[x, z];

                    ChunkNodeGrids[x, z] = new NodeGrid();
                    ChunkNodeGrids[x, z].grid = new PathNode[currentChunk.MaxX, currentChunk.MaxY, currentChunk.MaxZ];
                    foreach (Block block in currentChunk.GetWalkableBlocks())
                    {
                        ChunkNodeGrids[x, z].grid[block.x, block.y, block.z] = new PathNode(block);
                    }
                }
            }
        }
    }

}
