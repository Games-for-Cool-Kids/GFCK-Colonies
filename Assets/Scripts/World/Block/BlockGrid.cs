using UnityEngine;
using System.Collections.Generic;

public class BlockGrid
{
    public enum Adjacency
    {
        NORTH,
        SOUTH,
        WEST,
        EAST,
        ABOVE,
        BELOW,
    }

    public int MaxX { get; private set; }
    public int MaxY { get; private set; }
    public int MaxZ { get; private set; }

    private BlockData[,,] _blocks; // Data

    public BlockGrid(int maxX, int maxY, int maxZ)
    {
        MaxX = maxX;
        MaxY = maxY;
        MaxZ = maxZ;

        _blocks = new BlockData[MaxX, MaxY, MaxZ];
    }

    public void SetBlock(BlockData block)
    {
        _blocks[block.x, block.y, block.z] = block;
    }

    public BlockData GetBlock(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0
        || x >= MaxX || y >= MaxY || z >= MaxZ)
            return null;

        return _blocks[x, y, z];
    }

    public BlockData GetSurfaceBlock(int x, int z)
    {
        if (x < 0 || z < 0
        || x >= MaxX || z >= MaxZ)
            return null;

        for (int y = MaxY - 1; y > 0; y--) // Search from top-down until we hit a surface block.
        {
            BlockData block = GetBlock(x, y, z);
            if(block != null
            && block.filled)
                return block;
        }

        return null;
    }

    public BlockData GetAdjacentBlock(BlockData sourceBlock, Adjacency adjacency, bool checkVertically = false)
    {
        int x = sourceBlock.x;
        int y = sourceBlock.y;
        int z = sourceBlock.z;

        switch (adjacency)
        {
            case Adjacency.NORTH:
                z = z + 1; break;
            case Adjacency.SOUTH:
                z = z - 1; break;
            case Adjacency.WEST:
                x = x - 1; break;
            case Adjacency.EAST:
                x = x + 1; break;
            case Adjacency.ABOVE:
                y = y + 1; break;
            case Adjacency.BELOW:
                y = y - 1; break;
        }

        BlockData adjacentBlock = GetBlock(x, y, z);
        if (adjacentBlock != null)
            return adjacentBlock;

        if (checkVertically) // Check vertically if not found on same level.
        {
            // Check one above
            adjacentBlock = GetBlock(x, y + 1, z);
            if (adjacentBlock != null)
                return adjacentBlock;

            // Check one below
            adjacentBlock = GetBlock(x, y - 1, z);
            if (adjacentBlock != null)
                return adjacentBlock;
        }

        return null;
    }

    public List<BlockData> GetSurfaceNeighbors(int x, int y, int z, bool diagonal = true)
    {
        BlockData block = GetBlock(x, y, z);
        if(block != null)
            return GetSurfaceNeighbors(block);

        return new List<BlockData>(); // empty list
    }

    public List<BlockData> GetSurfaceNeighbors(BlockData source, bool diagonal = true)
    {
        List<BlockData> neighbors = new List<BlockData>();

        for (int x = source.x - 1; x <= source.x + 1; x++)
        {
            for (int z = source.z - 1; z <= source.z + 1; z++)
            {
                if (x == source.x && z == source.z) // Skip self.
                    continue;

                for (int y = MaxY - 1; y > 0; y--) // Search from top-down until we hit a surface block.
                {
                    if (!diagonal // Skip diagonal blocks.
                     && Mathf.Abs(x - source.x) == 1
                     && Mathf.Abs(z - source.z) == 1)
                        continue;

                    BlockData neighbor = GetBlock(x, y, z);
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

    public List<BlockData> GetFilledBlocks()
    {
        List<BlockData> filledBlocks = new();

        for (int x = 0; x < MaxX; x++)
        {
            for (int y = 0; y < MaxY; y++)
            {
                for (int z = 0; z < MaxZ; z++)
                {
                    BlockData block = _blocks[x, y, z];
                    if (block != null
                    && block.filled)
                        filledBlocks.Add(GetBlock(x, y, z));
                }
            }
        }

        return filledBlocks;
    }

    public BlockData[,,] GetData()
    {
        return _blocks;
    }

    public void SetData(BlockData[,,] data)
    {
        _blocks = data;
    }

    public void InsertGridAt(int posX, int posZ, BlockGrid other)
    {
        if (posX < 0 || posZ < 0
        || posX + other.MaxX > MaxX || other.MaxY > MaxY || posZ + other.MaxZ > MaxZ)
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
                    _blocks[x, y, z] = other.GetBlock(x, y, z);
                }
            }
        }
    }

    public void DestroyBlock(BlockData block)
    {
        if (block.y == 0) // Cannot destroy bottom-most block.
            return;

        int x = block.x;
        int y = block.y;
        int z = block.z;

        _blocks[x, y, z] = null; // Clear block in grid.

        BlockData belowBlock = GetBlock(x, y - 1, z);
        if (belowBlock == null) // Create block under destroyed block, if empty below.
        { 
            BlockData newBlock = BlockCode.CreateBlockData(x, y - 1, z, true, block.type, new Vector3(block.worldPosition.x, y - 1, block.worldPosition.z));
            SetBlock(newBlock);
        }

        // Fill holes at neighbors.
        var neighbors = GetSurfaceNeighbors(block, false);
        foreach (BlockData neighbor in neighbors)
        {
            int heightDiff = neighbor.y - (y - 1);
            CreateBlocksUnder(neighbor, heightDiff - 1);
        }
    }

    public void CreateBlocksUnder(BlockData block, int amount, BlockType type = BlockType.ROCK)
    {
        int x = block.x;
        int z = block.z;

        for (int y = block.y - 1; y >= block.y - amount; y--)
        {
            BlockData newBlock = BlockCode.CreateBlockData(x, y, z,
                                       true,
                                       type,
                                       new Vector3(block.worldPosition.x, y, block.worldPosition.z));
            SetBlock(newBlock);
        }
    }
}
