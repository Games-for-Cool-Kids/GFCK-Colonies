using UnityEngine;

public class ChunkGrid
{
    public int width;
    public int chunkSize;
    public ChunkData[,] chunks;

    public ChunkGrid(int width, int chunkSize)
    {
        this.width = width;
        this.chunkSize = chunkSize;
        chunks = new ChunkData[width, width];
    }

    public ChunkData GetChunk(int x, int z)
    {
        if (x < 0 || x >= width
        || z < 0 || z >= width)
            return null;

        return chunks[x, z];
    }

    public void FillNeighboringEdge(int x, int z, BlockAdjacency direction)
    {
        ChunkData current = chunks[x, z];
        ChunkData neighbor = null;
        switch (direction)
        {
            case BlockAdjacency.NORTH:
                neighbor = GetChunk(x, z + 1);
                break;
            case BlockAdjacency.SOUTH:
                neighbor = GetChunk(x, z - 1);
                break;
            case BlockAdjacency.EAST:
                neighbor = GetChunk(x + 1, z);
                break;
            case BlockAdjacency.WEST:
                neighbor = GetChunk(x - 1, z);
                break;
        }
        if (neighbor == null)
            return;


        for (int i = 0; i < chunkSize; i++)
        {
            BlockData currentBlock = null;
            BlockData neighborBlock = null;
            switch (direction)
            {
                case BlockAdjacency.NORTH:
                    currentBlock = ChunkCode.GetSurfaceBlock(current, i, chunkSize - 1);
                    neighborBlock = ChunkCode.GetSurfaceBlock(neighbor, i, 0);
                    break;
                case BlockAdjacency.SOUTH:
                    currentBlock = ChunkCode.GetSurfaceBlock(current, i, 0);
                    neighborBlock = ChunkCode.GetSurfaceBlock(neighbor, i, chunkSize - 1);
                    break;
                case BlockAdjacency.EAST:
                    currentBlock = ChunkCode.GetSurfaceBlock(current, chunkSize - 1, i);
                    neighborBlock = ChunkCode.GetSurfaceBlock(neighbor, 0, i);
                    break;
                case BlockAdjacency.WEST:
                    currentBlock = ChunkCode.GetSurfaceBlock(current, 0, i);
                    neighborBlock = ChunkCode.GetSurfaceBlock(neighbor, chunkSize - 1, i);
                    break;
            }

            int blocksToFill = currentBlock.y - neighborBlock.y - 1;
            if (blocksToFill > 0)
            {
                for (int y = currentBlock.y - 1; y >= currentBlock.y - blocksToFill; y--)
                {
                    BlockData fill = BlockCode.CreateBlockData(currentBlock.x, y, currentBlock.z, true, BlockType.ROCK, new Vector3(currentBlock.x, y, currentBlock.z));
                    ChunkCode.SetBlock(current, fill);
                }
            }
        }
    }

    public void DestroyBlock(BlockData block)
    {
        ChunkData chunk = GetChunkAt(block.worldPosition);
        ChunkCode.DestroyBlock(chunk, block);
        ChunkCode.CreateMeshData(chunk); // Update mesh.

        if(block.x == 0 && chunk.x > 0)
        {
            ChunkData westNeighbor = chunks[chunk.x - 1, chunk.z];
            BlockData neighborBlock = ChunkCode.GetSurfaceBlock(westNeighbor, chunkSize - 1, block.z);
            ChunkCode.CreateBlocksUnder(westNeighbor, neighborBlock, neighborBlock.y - block.y);
            ChunkCode.CreateMeshData(westNeighbor); // Update mesh.
        }
        else if (block.x == chunkSize - 1 && chunk.x < width - 1)
        {
            ChunkData eastNeighbor = chunks[chunk.x + 1, chunk.z];
            BlockData neighborBlock = ChunkCode.GetSurfaceBlock(eastNeighbor, 0, block.z);
            ChunkCode.CreateBlocksUnder(eastNeighbor, neighborBlock, neighborBlock.y - block.y);
            ChunkCode.CreateMeshData(eastNeighbor); // Update mesh.
        }

        if (block.z == 0 && chunk.z > 0)
        {
            ChunkData southNeighbor = chunks[chunk.x, chunk.z - 1];
            BlockData neighborBlock = ChunkCode.GetSurfaceBlock(southNeighbor, block.x, chunkSize - 1);
            ChunkCode.CreateBlocksUnder(southNeighbor, neighborBlock, neighborBlock.y - block.y);
            ChunkCode.CreateMeshData(southNeighbor); // Update mesh.
        }
        else if (block.z == chunkSize - 1 && chunk.z < width - 1)
        {
            ChunkData northNeighbor = chunks[chunk.x, chunk.z + 1];
            BlockData neighborBlock = ChunkCode.GetSurfaceBlock(northNeighbor, block.x, 0);
            ChunkCode.CreateBlocksUnder(northNeighbor, neighborBlock, neighborBlock.y - block.y);
            ChunkCode.CreateMeshData(northNeighbor); // Update mesh.
        }
    }

    public ChunkData GetChunkAt(Vector3 worldPos)
    {
        Vector3 relativePos = worldPos + new Vector3(0.5f, 0.5f, 0.5f); // We need offset of half a block. Origin is middle of first block.

        int chunkX = Mathf.FloorToInt(relativePos.x / this.chunkSize);
        int chunkZ = Mathf.FloorToInt(relativePos.z / this.chunkSize);

        return GetChunk(chunkX, chunkZ);
    }
}
