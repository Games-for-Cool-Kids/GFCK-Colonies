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

    public static void CreateMeshData(ChunkData[,] chunks, World.ChunkDimensions dimensions, ChunkData chunk)
    {
        chunk.meshData = new ChunkMeshData();
        chunk.meshChanged = true;

        foreach (BlockData filledBlock in GetFilledBlocks(chunk))
        {
            CreateBlockMesh(chunks, dimensions, chunk, filledBlock);
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

    public static bool IsSolidBlock(BlockData block)
    {
        return block != null && block.type != BlockType.AIR;
    }

    public static void CreateBlockMesh(ChunkData[,] chunks, World.ChunkDimensions dimensions, ChunkData chunk, BlockData block)
    {
        List<BlockAdjacency> sidesToCreate = new();
        foreach (var direction in BlockCode.cardinalDirections)
        {
            if (!HasNeighbor(chunks, dimensions, block.worldPosition, direction))
                sidesToCreate.Add(direction);
        }

        //if (HasNeighbor(_chunks, dimensions, block.worldPosition, BlockAdjacency.ABOVE))
        //{
        //    ChunkMeshUtilities.CreateBlock(chunk.meshData, BlockCode.GetLocalPosition(block), sidesToCreate, block.type);
        //    return;
        //}
        //
        //if (sidesToCreate.Count == 1)
        //{
        //    if (HasNeighbor(_chunks, dimensions, block.worldPosition + Vector3.down, sidesToCreate[0]))
        //    {
        //        ChunkMeshUtilities.CreateSlopeBlock(chunk.meshData, BlockCode.GetLocalPosition(block), sidesToCreate[0], block.type);
        //        return;
        //    }
        //}
        //else if (sidesToCreate.Count == 2)
        //{
        //    BlockAdjacency ordinal = BlockCode.GetOrdinalDirection(sidesToCreate[0], sidesToCreate[1]);
        //    if (BlockCode.ordinalDirections.Contains(ordinal)
        //     && HasNeighbor(_chunks, dimensions, block.worldPosition + Vector3.down, sidesToCreate[0])
        //     && HasNeighbor(_chunks, dimensions, block.worldPosition + Vector3.down, sidesToCreate[1]))
        //    {
        //        ChunkMeshUtilities.CreateCornerSlopeBlock(chunk.meshData, BlockCode.GetLocalPosition(block), BlockCode.GetOrdinalDirection(sidesToCreate[0], sidesToCreate[1]), block.type);
        //        return;
        //    }
        //}

        if (!HasNeighbor(chunks, dimensions, block.worldPosition, BlockAdjacency.ABOVE))
            sidesToCreate.Add(BlockAdjacency.ABOVE);

        ChunkMeshUtilities.CreateBlock(chunk.meshData, BlockCode.GetLocalPosition(block), sidesToCreate, block.type);
    }

    public static bool HasNeighbor(ChunkData[,] chunks, World.ChunkDimensions dimensions, Vector3 blockPos, BlockAdjacency direction)
    {
        Vector3 offset = Vector3.zero;
        switch (direction)
        {
            case BlockAdjacency.NORTH:
                offset = Vector3.forward;
                break;
            case BlockAdjacency.SOUTH:
                offset = Vector3.back;
                break;
            case BlockAdjacency.WEST:
                offset = Vector3.left;
                break;
            case BlockAdjacency.EAST:
                offset = Vector3.right;
                break;
            case BlockAdjacency.ABOVE:
                offset = Vector3.up;
                break;
            case BlockAdjacency.BELOW:
                offset = Vector3.down;
                break;
        }

        var neighbor = GetBlockAt(chunks, dimensions, blockPos + offset);
        return IsSolidBlock(neighbor);
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

    public static BlockData GetBlockAt(ChunkData chunk, Vector3 worldPos)
    {
        worldPos -= chunk.origin; // Make relative to chunk.

        int blockX = Mathf.RoundToInt(worldPos.x);
        int blockY = Mathf.RoundToInt(worldPos.y);
        int blockZ = Mathf.RoundToInt(worldPos.z);

        return GetBlock(chunk, blockX, blockY, blockZ);
    }

    public static BlockData GetBlockAt(ChunkData[,] chunks, World.ChunkDimensions dimensions, Vector3 worldPos)
    {
        ChunkData chunk = GetChunkAt(chunks, dimensions, worldPos);

        if (chunk == null)
            return null;

        return GetBlockAt(chunk, worldPos);
    }

    public static BlockData GetSurfaceBlockUnder(ChunkData[,] chunks, World.ChunkDimensions dimensions, Vector3 worldPos)
    {
        ChunkData chunk = GetChunkAt(chunks, dimensions, worldPos);

        if (chunk == null)
            return null;

        Vector3 localBlockPos = worldPos - chunk.origin;
        int x = Mathf.FloorToInt(localBlockPos.x);
        int startY = Mathf.FloorToInt(worldPos.y); // Start at given y, in case there is overlap.
        int z = Mathf.FloorToInt(localBlockPos.z);

        for (int y = startY; y > 0; y--) // Search from top-down until we hit a surface block.
        {
            BlockData block = GetBlock(chunk, x, y, z);
            if (IsSolidBlock(block))
                return block;
        }

        return null;

    }

    public static BlockData GetSurfaceBlock(ChunkData chunk, int x, int z)
    {
        if (x < 0 || z < 0
        || x >= chunk.MaxX || z >= chunk.MaxZ)
            return null;

        for (int y = chunk.MaxY - 1; y > 0; y--) // Search from top-down until we hit a surface block.
        {
            BlockData block = GetBlock(chunk, x, y, z);
            if (IsSolidBlock(block))
                return block;
        }

        return null;
    }

    public static BlockData GetAdjacentBlock(ChunkData chunk, BlockData sourceBlock, BlockAdjacency adjacency)
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

        return GetBlock(chunk, x, y, z);
    }

    public static ChunkData GetChunk(ChunkData[,] chunks, int worldChunkWidth, int x, int z)
    {
        if (x < 0 || x >= worldChunkWidth
        || z < 0 || z >= worldChunkWidth)
            return null;

        return chunks[x, z];
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
                    if (IsSolidBlock(neighbor))
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
                    if (IsSolidBlock(block))
                        filledBlocks.Add(GetBlock(chunk, x, y, z));
                }
            }
        }

        return filledBlocks;
    }

    public static List<BlockData> GetWalkableBlocks(ChunkData chunk)
    {
        List<BlockData> walkableBlocks = new();

        for (int x = 0; x < chunk.MaxX; x++)
        {
            for (int y = 0; y < chunk.MaxY; y++)
            {
                for (int z = 0; z < chunk.MaxZ; z++)
                {
                    BlockData block = chunk.blocks[x, y, z];
                    if (block != null
                     && IsWalkable(chunk, block))
                        walkableBlocks.Add(GetBlock(chunk, x, y, z));
                }
            }
        }

        return walkableBlocks;
    }

    public static void CreateBlocksUnder(ChunkData chunk, BlockData block, int amount, BlockType type = BlockType.ROCK)
    {
        int x = block.x;
        int z = block.z;

        for (int y = block.y - 1; y >= block.y - amount; y--)
        {
            BlockData newBlock = BlockCode.CreateBlockData(x, y, z,
                                       type,
                                       new Vector3(block.worldPosition.x, y, block.worldPosition.z));
            SetBlock(chunk, newBlock);
        }
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
                    BlockData fill = BlockCode.CreateBlockData(currentBlock.x, y, currentBlock.z, BlockType.ROCK, new Vector3(currentBlock.x, y, currentBlock.z));
                    ChunkCode.SetBlock(current, fill);
                }
            }
        }
    }

    public static void AddBlock(ChunkData[,] chunks, World.ChunkDimensions dimensions, Vector3 worldPos)
    {
        ChunkData chunk = GetChunkAt(chunks, dimensions, worldPos);

        Vector3 localPos = worldPos - chunk.origin;
        int x = Mathf.FloorToInt(localPos.x);
        int y = Mathf.FloorToInt(localPos.y);
        int z = Mathf.FloorToInt(localPos.z);

        BlockData newBlock = BlockCode.CreateBlockData(localPos, BlockType.GROUND, worldPos);
        chunk.blocks[x, y, z] = newBlock;

        ChunkCode.CreateMeshData(chunks, dimensions, chunk); // Update mesh.

        GameManager.Instance.World.InvokeBlockAddEvent(newBlock);
    }

    public static void DigBlock(ChunkData chunk, BlockData block)
    {
        if (block.y == 0) // Cannot destroy bottom-most block.
            return;

        int x = block.x;
        int y = block.y;
        int z = block.z;

        chunk.blocks[x, y, z].type = BlockType.AIR; // Set block to empty.

        GameManager.Instance.World.InvokeBlockDigEvent(block);
    }

    public static void DigBlock(ChunkData[,] chunks, World.ChunkDimensions dimensions, BlockData block)
    {
        ChunkData chunk = GetChunkAt(chunks, dimensions, block.worldPosition);
        DigBlock(chunk, block);
        CreateMeshData(chunks, dimensions, chunk); // Update mesh.
    }

    public static ChunkData GetChunkAt(ChunkData[,] chunks, World.ChunkDimensions dimensions, Vector3 worldPos)
    {
        Vector3 relativePos = worldPos + new Vector3(0.5f, 0.5f, 0.5f); // We need offset of half a block. Origin is middle of first block.

        int chunkX = Mathf.FloorToInt(relativePos.x / dimensions.chunkSize);
        int chunkZ = Mathf.FloorToInt(relativePos.z / dimensions.chunkSize);

        return GetChunk(chunks, dimensions.worldChunkWidth, chunkX, chunkZ);
    }

    public static bool IsSurfaceBlock(ChunkData chunk, BlockData block)
    {
        BlockData blockAbove = GetAdjacentBlock(chunk, block, BlockAdjacency.ABOVE);
        return blockAbove == null
            || blockAbove.type == BlockType.AIR;
    }

    public static bool IsWalkable(ChunkData chunk, BlockData block)
    {
        return IsSurfaceBlock(chunk, block)
            && block.type != BlockType.AIR
            && block.type != BlockType.WATER
            && block.passable; // Determines if it can be used for pathfinding.
    }

    public static List<BlockData> GetSurroundingBlocks(ChunkData[,] chunks, World.ChunkDimensions dimensions, Vector3 worldPos, bool diagonal = true, bool includeAir = false)
    {
        List<BlockData> neighbors = new List<BlockData>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0) // Skip self.
                    continue;

                if (!diagonal // Skip diagonal blocks.
                 && Mathf.Abs(x) == 1
                 && Mathf.Abs(z) == 1)
                    continue;

                Vector3 searchPos = worldPos + new Vector3(x, 0, z);
                BlockData neighbor = GetBlockAt(chunks, dimensions, searchPos);
                if (neighbor != null)
                {
                    if (!includeAir
                     && neighbor.type == BlockType.AIR)
                        continue;

                    neighbors.Add(neighbor);
                }
            }
        }
        return neighbors;
    }
}
