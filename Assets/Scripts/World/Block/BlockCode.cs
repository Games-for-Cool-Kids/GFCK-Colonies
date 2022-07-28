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
    public static BlockData CreateBlockData(Vector3 localPosition, BlockType type, Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(localPosition.x);
        int y = Mathf.FloorToInt(localPosition.y);
        int z = Mathf.FloorToInt(localPosition.z);

        return CreateBlockData(x, y, z, type, worldPosition);
    }

    public static BlockData CreateBlockData(int x, int y, int z, BlockType type, Vector3 worldPosition)
    {
        BlockData data = new();
        data.x = x;
        data.y = y;
        data.z = z;
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
}
