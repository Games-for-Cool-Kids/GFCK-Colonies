using UnityEngine;
using System.Collections.Generic;

public class ChunkCode
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

    public static BlockData GetBlockAt(ChunkData chunk, Vector3 worldPos)
    {
        worldPos -= chunk.origin; // Make relative to chunk.

        int blockX = Mathf.RoundToInt(worldPos.x);
        int blockY = Mathf.RoundToInt(worldPos.y);
        int blockZ = Mathf.RoundToInt(worldPos.z);

        return GetBlock(chunk, blockX, blockY, blockZ);
    }

    public static void DestroyBlock(ChunkData chunk, BlockData block)
    {
        if (block.y == 0) // Cannot destroy bottom-most block.
            return;

        int x = block.x;
        int y = block.y;
        int z = block.z;

        chunk.blocks[x, y, z] = null; // Clear block in grid.

        BlockData belowBlock = GetBlock(chunk, x, y - 1, z);
        if (belowBlock == null) // Create block under destroyed block, if empty below.
        {
            BlockData newBlock = BlockCode.CreateBlockData(x, y - 1, z, true, block.type, new Vector3(block.worldPosition.x, y - 1, block.worldPosition.z));
            SetBlock(chunk, newBlock);
        }

        // Fill holes at neighbors.
        var neighbors = GetSurfaceNeighbors(chunk, block, false);
        foreach (BlockData neighbor in neighbors)
        {
            int heightDiff = neighbor.y - (y - 1);
            CreateBlocksUnder(chunk, neighbor, heightDiff - 1);
        }
    }

    public static void CreateMeshData(ChunkData chunk)
    {
        chunk.meshData = new ChunkMeshData();
        chunk.meshChanged = true;

        foreach (BlockData filledBlock in GetFilledBlocks(chunk))
        {
            AddBlockToChunkMesh(chunk, filledBlock);
        }
    }

    public static Mesh TakeMesh(ChunkData chunk)
    {
        chunk.meshChanged = false;

        Mesh chunkMesh = new Mesh()
        {
            vertices = chunk.meshData.vertices.ToArray(),
            uv = chunk.meshData.uv.ToArray(),
            triangles = chunk.meshData.triangles.ToArray()
        };
        chunkMesh.RecalculateNormals();

        return chunkMesh;
    }

    public static void CreateBlocksUnder(ChunkData chunk, int x, int y, int z, int amount)
    {
        CreateBlocksUnder(chunk, GetBlock(chunk, x, y, z), amount);
    }

    public static void AddBlockToChunkMesh(ChunkData chunk, BlockData block)
    {
        //Vector3 localPos = BlockCode.GetLocalPosition(block);

        //var neighbours = GetAllNeighboringBlocks(chunk, block);
 
        // Flat faces first
        //if (IsSolidBlock(neighbours[BlockAdjacency.NORTH]))
        {
            //ChunkMeshUtilities.CreateFaceForward(chunk.meshData, localPos, block.type);
        }

        AddBlockFaceToMeshIfVisible(chunk, block, BlockAdjacency.ABOVE);
        AddBlockFaceToMeshIfVisible(chunk, block, BlockAdjacency.NORTH);
        AddBlockFaceToMeshIfVisible(chunk, block, BlockAdjacency.SOUTH);
        AddBlockFaceToMeshIfVisible(chunk, block, BlockAdjacency.EAST);
        AddBlockFaceToMeshIfVisible(chunk, block, BlockAdjacency.WEST);
    }

    public static bool IsSolidBlock(BlockData block)
    {
        return block != null && block.filled;
    }

    private static void AddBlockFaceToMeshIfVisible(ChunkData chunk, BlockData block, BlockAdjacency adjacency)
    {
        BlockData neighbor = GetAdjacentBlock(chunk, block, adjacency);
        if (neighbor == null
        || !neighbor.filled)
        {
            Vector3 localPos = BlockCode.GetLocalPosition(block);

            switch (adjacency)
            {
                case BlockAdjacency.NORTH:
                    ChunkMeshUtilities.CreateFaceForward(chunk.meshData, localPos, block.type);
                    break;
                case BlockAdjacency.SOUTH:
                    ChunkMeshUtilities.CreateFaceBackward(chunk.meshData, localPos, block.type);
                    break;
                case BlockAdjacency.EAST:
                    ChunkMeshUtilities.CreateFaceRight(chunk.meshData, localPos, block.type);
                    break;
                case BlockAdjacency.WEST:
                    ChunkMeshUtilities.CreateFaceLeft(chunk.meshData, localPos, block.type);
                    break;
                case BlockAdjacency.ABOVE:
                    ChunkMeshUtilities.CreateFaceUp(chunk.meshData, localPos, block.type);
                    break;
                case BlockAdjacency.BELOW:  // Maybe we can even skip the backside of blocks too.
                default:                         // We don't create a bottom face. Since the camera can never see the bottom of blocks.
                    break;
            }
        }
    }

    public static void SetBlock(ChunkData chunk, BlockData block)
    {
        chunk.blocks[block.x, block.y, block.z] = block;
    }

    public static BlockData GetBlock(ChunkData chunk, int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0
        || x >= chunk.MaxX || y >= chunk.MaxY || z >= chunk.MaxZ)
            return null;

        return chunk.blocks[x, y, z];
    }

    public static BlockData GetSurfaceBlock(ChunkData chunk, int x, int z)
    {
        if (x < 0 || z < 0
        || x >= chunk.MaxX || z >= chunk.MaxZ)
            return null;

        for (int y = chunk.MaxY - 1; y > 0; y--) // Search from top-down until we hit a surface block.
        {
            BlockData block = GetBlock(chunk, x, y, z);
            if (block != null
            && block.filled)
                return block;
        }

        return null;
    }

    public static Dictionary<BlockAdjacency, BlockData> GetAllNeighboringBlocks(ChunkData chunk, BlockData sourceBlock)
    {
        int x = sourceBlock.x;
        int y = sourceBlock.y;
        int z = sourceBlock.z;

        Dictionary<BlockAdjacency, BlockData> neighbours = new Dictionary<BlockAdjacency, BlockData>()
        {
            { BlockAdjacency.NORTH, GetBlock(chunk, x, y, z + 1) },
            { BlockAdjacency.SOUTH, GetBlock(chunk, x, y, z - 1) },
            { BlockAdjacency.WEST, GetBlock(chunk, x - 1, y, z) },
            { BlockAdjacency.EAST, GetBlock(chunk, x + 1, y, z) },
            { BlockAdjacency.ABOVE, GetBlock(chunk, x, y + 1, z) },
            { BlockAdjacency.BELOW, GetBlock(chunk, x, y - 1, z) },
            { BlockAdjacency.NORTH_BELOW, GetBlock(chunk, x, y - 1, z + 1) },
            { BlockAdjacency.SOUTH_BELOW, GetBlock(chunk, x, y - 1, z - 1) },
            { BlockAdjacency.WEST_BELOW, GetBlock(chunk, x - 1, y - 1, z) },
            { BlockAdjacency.EAST_BELOW, GetBlock(chunk, x + 1, y - 1, z) },
            { BlockAdjacency.NORTH_ABOVE, GetBlock(chunk, x, y + 1, z + 1) },
            { BlockAdjacency.SOUTH_ABOVE, GetBlock(chunk, x, y + 1, z - 1) },
            { BlockAdjacency.WEST_ABOVE, GetBlock(chunk, x - 1, y + 1, z) },
            { BlockAdjacency.EAST_ABOVE, GetBlock(chunk, x + 1, y + 1, z) },
        };

        return neighbours;
    }

    public static BlockData GetAdjacentBlock(ChunkData chunk, BlockData sourceBlock, BlockAdjacency adjacency, bool checkVertically = false)
    {
        int x = sourceBlock.x;
        int y = sourceBlock.y;
        int z = sourceBlock.z;

        switch (adjacency)
        {
            case BlockAdjacency.NORTH:
                z = z + 1; break;
            case BlockAdjacency.SOUTH:
                z = z - 1; break;
            case BlockAdjacency.WEST:
                x = x - 1; break;
            case BlockAdjacency.EAST:
                x = x + 1; break;
            case BlockAdjacency.ABOVE:
                y = y + 1; break;
            case BlockAdjacency.BELOW:
                y = y - 1; break;
        }

        BlockData adjacentBlock = GetBlock(chunk, x, y, z);
        if (adjacentBlock != null)
            return adjacentBlock;

        if (checkVertically) // Check vertically if not found on same level.
        {
            // Check one above
            adjacentBlock = GetBlock(chunk, x, y + 1, z);
            if (adjacentBlock != null)
                return adjacentBlock;

            // Check one below
            adjacentBlock = GetBlock(chunk, x, y - 1, z);
            if (adjacentBlock != null)
                return adjacentBlock;
        }

        return null;
    }

    public static List<BlockData> GetSurfaceNeighbors(ChunkData chunk, int x, int y, int z, bool diagonal = true)
    {
        BlockData block = GetBlock(chunk, x, y, z);
        if (block != null)
            return GetSurfaceNeighbors(chunk, block);

        return new List<BlockData>(); // empty list
    }

    public static List<BlockData> GetSurfaceNeighbors(ChunkData chunk, BlockData source, bool diagonal = true)
    {
        List<BlockData> neighbors = new List<BlockData>();

        for (int x = source.x - 1; x <= source.x + 1; x++)
        {
            for (int z = source.z - 1; z <= source.z + 1; z++)
            {
                if (x == source.x && z == source.z) // Skip self.
                    continue;

                for (int y = chunk.MaxY - 1; y > 0; y--) // Search from top-down until we hit a surface block.
                {
                    if (!diagonal // Skip diagonal blocks.
                     && Mathf.Abs(x - source.x) == 1
                     && Mathf.Abs(z - source.z) == 1)
                        continue;

                    BlockData neighbor = GetBlock(chunk, x, y, z);
                    if (neighbor != null
                     && neighbor.filled)
                    {
                        neighbors.Add(neighbor);
                        y = 0; // Stop top-down search.
                    }
                }
            }
        }

        return neighbors;
    }

    public static List<BlockData> GetFilledBlocks(ChunkData chunk)
    {
        List<BlockData> filledBlocks = new();

        for (int x = 0; x < chunk.MaxX; x++)
        {
            for (int y = 0; y < chunk.MaxY; y++)
            {
                for (int z = 0; z < chunk.MaxZ; z++)
                {
                    BlockData block = chunk.blocks[x, y, z];
                    if (block != null
                    && block.filled)
                        filledBlocks.Add(GetBlock(chunk, x, y, z));
                }
            }
        }

        return filledBlocks;
    }

    public static BlockData[,,] GetData(ChunkData chunk)
    {
        return chunk.blocks;
    }

    public static void SetData(ChunkData chunk, BlockData[,,] data)
    {
        chunk.blocks = data;
    }

    public static void InsertGridAt(ChunkData chunk, ChunkData other, int posX, int posZ)
    {
        if (posX < 0 || posZ < 0
        || posX + other.MaxX > chunk.MaxX || other.MaxY > chunk.MaxY || posZ + other.MaxZ > chunk.MaxZ)
        {
            Debug.LogError("Out of bounds.");
            return;
        }

        for (int x = posX; x < posX + other.MaxX; x++)
        {
            for (int y = 0; y < other.MaxY; y++)
            {
                for (int z = posZ; z < posZ + other.MaxZ; z++)
                {
                    chunk.blocks[x, y, z] = GetBlock(other, x, y, z);
                }
            }
        }
    }

    public static void CreateBlocksUnder(ChunkData chunk, BlockData block, int amount, BlockType type = BlockType.ROCK)
    {
        int x = block.x;
        int z = block.z;

        for (int y = block.y - 1; y >= block.y - amount; y--)
        {
            BlockData newBlock = BlockCode.CreateBlockData(x, y, z,
                                       true,
                                       type,
                                       new Vector3(block.worldPosition.x, y, block.worldPosition.z));
            SetBlock(chunk, newBlock);
        }
    }

    public static ChunkData GetChunk(ChunkData[,] chunks, int worldChunkWidth, int x, int z)
    {
        if (x < 0 || x >= worldChunkWidth
        || z < 0 || z >= worldChunkWidth)
            return null;

        return chunks[x, z];
    }

    public static void FillNeighboringEdge(ChunkData[,] chunks, World.ChunkDimensions dimensions, int x, int z, BlockAdjacency direction)
    {
        ChunkData current = chunks[x, z];
        ChunkData neighbor = null;
        switch (direction)
        {
            case BlockAdjacency.NORTH:
                neighbor = GetChunk(chunks, dimensions.worldChunkWidth, x, z + 1);
                break;
            case BlockAdjacency.SOUTH:
                neighbor = GetChunk(chunks, dimensions.worldChunkWidth, x, z - 1);
                break;
            case BlockAdjacency.EAST:
                neighbor = GetChunk(chunks, dimensions.worldChunkWidth, x + 1, z);
                break;
            case BlockAdjacency.WEST:
                neighbor = GetChunk(chunks, dimensions.worldChunkWidth, x - 1, z);
                break;
        }
        if (neighbor == null)
            return;


        for (int i = 0; i < dimensions.chunkSize; i++)
        {
            BlockData currentBlock = null;
            BlockData neighborBlock = null;
            switch (direction)
            {
                case BlockAdjacency.NORTH:
                    currentBlock = ChunkCode.GetSurfaceBlock(current, i, dimensions.chunkSize - 1);
                    neighborBlock = ChunkCode.GetSurfaceBlock(neighbor, i, 0);
                    break;
                case BlockAdjacency.SOUTH:
                    currentBlock = ChunkCode.GetSurfaceBlock(current, i, 0);
                    neighborBlock = ChunkCode.GetSurfaceBlock(neighbor, i, dimensions.chunkSize - 1);
                    break;
                case BlockAdjacency.EAST:
                    currentBlock = ChunkCode.GetSurfaceBlock(current, dimensions.chunkSize - 1, i);
                    neighborBlock = ChunkCode.GetSurfaceBlock(neighbor, 0, i);
                    break;
                case BlockAdjacency.WEST:
                    currentBlock = ChunkCode.GetSurfaceBlock(current, 0, i);
                    neighborBlock = ChunkCode.GetSurfaceBlock(neighbor, dimensions.chunkSize - 1, i);
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

    public static void DestroyBlock(ChunkData[,] chunks, World.ChunkDimensions dimensions, BlockData block)
    {
        ChunkData chunk = GetChunkAt(chunks, dimensions, block.worldPosition);
        ChunkCode.DestroyBlock(chunk, block);
        ChunkCode.CreateMeshData(chunk); // Update mesh.

        if (block.x == 0 && chunk.x > 0)
        {
            ChunkData westNeighbor = chunks[chunk.x - 1, chunk.z];
            BlockData neighborBlock = ChunkCode.GetSurfaceBlock(westNeighbor, dimensions.chunkSize - 1, block.z);
            ChunkCode.CreateBlocksUnder(westNeighbor, neighborBlock, neighborBlock.y - block.y);
            ChunkCode.CreateMeshData(westNeighbor); // Update mesh.
        }
        else if (block.x == dimensions.chunkSize - 1 && chunk.x < dimensions.worldChunkWidth - 1)
        {
            ChunkData eastNeighbor = chunks[chunk.x + 1, chunk.z];
            BlockData neighborBlock = ChunkCode.GetSurfaceBlock(eastNeighbor, 0, block.z);
            ChunkCode.CreateBlocksUnder(eastNeighbor, neighborBlock, neighborBlock.y - block.y);
            ChunkCode.CreateMeshData(eastNeighbor); // Update mesh.
        }

        if (block.z == 0 && chunk.z > 0)
        {
            ChunkData southNeighbor = chunks[chunk.x, chunk.z - 1];
            BlockData neighborBlock = ChunkCode.GetSurfaceBlock(southNeighbor, block.x, dimensions.chunkSize - 1);
            ChunkCode.CreateBlocksUnder(southNeighbor, neighborBlock, neighborBlock.y - block.y);
            ChunkCode.CreateMeshData(southNeighbor); // Update mesh.
        }
        else if (block.z == dimensions.chunkSize - 1 && chunk.z < dimensions.worldChunkWidth - 1)
        {
            ChunkData northNeighbor = chunks[chunk.x, chunk.z + 1];
            BlockData neighborBlock = ChunkCode.GetSurfaceBlock(northNeighbor, block.x, 0);
            ChunkCode.CreateBlocksUnder(northNeighbor, neighborBlock, neighborBlock.y - block.y);
            ChunkCode.CreateMeshData(northNeighbor); // Update mesh.
        }
    }

    public static ChunkData GetChunkAt(ChunkData[,] chunks, World.ChunkDimensions dimensions, Vector3 worldPos)
    {
        Vector3 relativePos = worldPos + new Vector3(0.5f, 0.5f, 0.5f); // We need offset of half a block. Origin is middle of first block.

        int chunkX = Mathf.FloorToInt(relativePos.x / dimensions.chunkSize);
        int chunkZ = Mathf.FloorToInt(relativePos.z / dimensions.chunkSize);

        return GetChunk(chunks, dimensions.worldChunkWidth, chunkX, chunkZ);
    }

}
