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
        AddBlockFaceToMeshIfVisible(chunk, block, BlockAdjacency.ABOVE);
        AddBlockFaceToMeshIfVisible(chunk, block, BlockAdjacency.NORTH);
        AddBlockFaceToMeshIfVisible(chunk, block, BlockAdjacency.SOUTH);
        AddBlockFaceToMeshIfVisible(chunk, block, BlockAdjacency.EAST);
        AddBlockFaceToMeshIfVisible(chunk, block, BlockAdjacency.WEST);
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
}
