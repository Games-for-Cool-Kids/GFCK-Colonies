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

    private Block[,,] _blocks; // Data

    public BlockGrid(int maxX, int maxY, int maxZ)
    {
        MaxX = maxX;
        MaxY = maxY;
        MaxZ = maxZ;

        _blocks = new Block[MaxX, MaxY, MaxZ];
    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        _blocks[x, y, z] = block;
    }

    public Block GetBlock(int x, int y, int z)
    {
        if(x < 0 || y < 0 || z < 0
        || x >= MaxX || y >= MaxY || z >= MaxZ )
            return null;

        return _blocks[x, y, z];
    }

    public Block GetAdjacentBlock(Block sourceBlock, Adjacency adjacency, bool checkVertically = false)
    {
        int x = sourceBlock.x;
        int y = sourceBlock.y;
        int z = sourceBlock.z;

        switch(adjacency)
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

        Block adjacentBlock = GetBlock(x, y, z);
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

    public List<Block> GetFilledBlocks()
    {
        List<Block> filledBlocks = new();

        for (int x = 0; x < MaxX; x++)
        {
            for (int y = 0; y < MaxY; y++)
            {
                for (int z = 0; z < MaxZ; z++)
                {
                    Block block = _blocks[x, y, z];
                    if(block != null
                    && block.filled)
                        filledBlocks.Add(GetBlock(x, y, z));
                }
            }
        }

        return filledBlocks;
    }

    public Block[,,] GetData()
    {
        return _blocks;
    }

    public void setData(Block[,,] data)
    {
        _blocks = data;
    }
}
