using UnityEngine;

public class ChunkGrid
{
    public int width;
    public int chunkSize;
    public Chunk[,] chunks;

    public ChunkGrid(int width, int chunkSize)
    {
        this.width = width;
        this.chunkSize = chunkSize;
        chunks = new Chunk[width, width];
    }

    public Chunk GetChunk(int x, int z)
    {
        if (x < 0 || x >= width
        || z < 0 || z >= width)
            return null;

        return chunks[x, z];
    }

    public void FillNeighboringEdge(int x, int z, BlockGrid.Adjacency direction)
    {
        Chunk current = chunks[x, z];
        Chunk neighbor = null;
        switch (direction)
        {
            case BlockGrid.Adjacency.NORTH:
                neighbor = GetChunk(x, z + 1);
                break;
            case BlockGrid.Adjacency.SOUTH:
                neighbor = GetChunk(x, z - 1);
                break;
            case BlockGrid.Adjacency.EAST:
                neighbor = GetChunk(x + 1, z);
                break;
            case BlockGrid.Adjacency.WEST:
                neighbor = GetChunk(x - 1, z);
                break;
        }
        if (neighbor == null)
            return;


        for (int i = 0; i < chunkSize; i++)
        {
            Block currentBlock = null;
            Block neighborBlock = null;
            switch (direction)
            {
                case BlockGrid.Adjacency.NORTH:
                    currentBlock = current.grid.GetSurfaceBlock(i, chunkSize - 1);
                    neighborBlock = neighbor.grid.GetSurfaceBlock(i, 0);
                    break;
                case BlockGrid.Adjacency.SOUTH:
                    currentBlock = current.grid.GetSurfaceBlock(i, 0);
                    neighborBlock = neighbor.grid.GetSurfaceBlock(i, chunkSize - 1);
                    break;
                case BlockGrid.Adjacency.EAST:
                    currentBlock = current.grid.GetSurfaceBlock(chunkSize - 1, i);
                    neighborBlock = neighbor.grid.GetSurfaceBlock(0, i);
                    break;
                case BlockGrid.Adjacency.WEST:
                    currentBlock = current.grid.GetSurfaceBlock(0, i);
                    neighborBlock = neighbor.grid.GetSurfaceBlock(chunkSize - 1, i);
                    break;
            }

            int blocksToFill = currentBlock.y - neighborBlock.y - 1;
            if (blocksToFill > 0)
            {
                for (int y = currentBlock.y - 1; y >= currentBlock.y - blocksToFill; y--)
                {
                    Block fill = new Block(currentBlock.x, y, currentBlock.z, true, Block.Type.ROCK, new Vector3(currentBlock.x, y, currentBlock.z));
                    current.grid.SetBlock(fill);
                }
            }
        }
    }

    public void DestroyBlock(Block block)
    {
        Chunk chunk = GetChunkAt(block.worldPosition);
        chunk.DestroyBlock(block);
        chunk.CreateMeshData(); // Update mesh.

        if(block.x == 0 && chunk.x > 0)
        {
            Chunk westNeighbor = chunks[chunk.x - 1, chunk.z];
            Block neighborBlock = westNeighbor.grid.GetSurfaceBlock(chunkSize - 1, block.z);
            westNeighbor.grid.CreateBlocksUnder(neighborBlock, neighborBlock.y - block.y);
            westNeighbor.CreateMeshData(); // Update mesh.
        }
        else if (block.x == chunkSize - 1 && chunk.x < width - 1)
        {
            Chunk eastNeighbor = chunks[chunk.x + 1, chunk.z];
            Block neighborBlock = eastNeighbor.grid.GetSurfaceBlock(0, block.z);
            eastNeighbor.grid.CreateBlocksUnder(neighborBlock, neighborBlock.y - block.y);
            eastNeighbor.CreateMeshData(); // Update mesh.
        }
        if (block.z == 0 && chunk.z > 0)
        {
            Chunk southNeighbor = chunks[chunk.x, chunk.z - 1];
            Block neighborBlock = southNeighbor.grid.GetSurfaceBlock(block.x, chunkSize - 1);
            southNeighbor.grid.CreateBlocksUnder(neighborBlock, neighborBlock.y - block.y);
            southNeighbor.CreateMeshData(); // Update mesh.
        }
        else if (block.z == chunkSize - 1 && chunk.z < width - 1)
        {
            Chunk northNeighbor = chunks[chunk.x, chunk.z + 1];
            Block neighborBlock = northNeighbor.grid.GetSurfaceBlock(block.x, 0);
            northNeighbor.grid.CreateBlocksUnder(neighborBlock, neighborBlock.y - block.y);
            northNeighbor.CreateMeshData(); // Update mesh.
        }
    }

    public Chunk GetChunkAt(Vector3 worldPos)
    {
        Vector3 relativePos = worldPos + new Vector3(0.5f, 0.5f, 0.5f); // We need offset of half a block. Origin is middle of first block.

        int chunkX = Mathf.FloorToInt(relativePos.x / this.chunkSize);
        int chunkZ = Mathf.FloorToInt(relativePos.z / this.chunkSize);

        return GetChunk(chunkX, chunkZ);
    }
}
