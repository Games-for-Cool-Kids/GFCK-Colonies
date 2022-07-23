using UnityEngine;

public enum BlockAdjacency
{
    NORTH,
    SOUTH,
    WEST,
    EAST,
    ABOVE,
    BELOW,
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
