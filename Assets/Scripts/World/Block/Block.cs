using UnityEngine;

public enum BlockType
{
    GROUND,
    GRASS,
    WATER,
    SAND,
    SNOW,
    ROCK,
}

public class BlockData
{
    public int x, y, z; // Local position in chunk.
    public bool filled;
    public Vector3 worldPosition;
    public BlockType type;
}

public class BlockCode
{
    public static BlockData CreateBlockData(int x, int y, int z, bool filled, BlockType type, Vector3 worldPosition)
    {
        BlockData data = new();
        data.x = x;
        data.y = y;
        data.z = z;
        data.filled = filled;
        data.worldPosition = worldPosition;
        data.type = type;

        return data;
    }

    public static Vector3 GetLocalPosition(BlockData block)
    {
        return new Vector3(block.x, block.y, block.z);
    }

    public static Vector3 GetSurfaceWorldPos(BlockData block)
    {
        return block.worldPosition + Vector3.up / 2;
    }

    public static bool IsWalkable(BlockData block)
    {
        return block.filled; // Determines if it can be used for pathfinding.
    }
}
