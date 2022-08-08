using UnityEngine;
using System.Collections.Generic;

public enum BlockAdjacency
{
    NORTH,
    SOUTH,
    WEST,
    EAST,
    ABOVE,
    BELOW,
    NORTHEAST,
    SOUTHEAST,
    SOUTHWEST,
    NORTHWEST,
}

public class BlockCode
{
    public static List<BlockAdjacency> cardinalDirections = GetCardinalDirections();
    public static List<BlockAdjacency> ordinalDirections = GetOrdinalDirections();

    private static List<BlockAdjacency> GetCardinalDirections()
    {
        List<BlockAdjacency> directions = new();
        directions.Add(BlockAdjacency.NORTH);
        directions.Add(BlockAdjacency.EAST);
        directions.Add(BlockAdjacency.SOUTH);
        directions.Add(BlockAdjacency.WEST);

        return directions;
    }
    private static List<BlockAdjacency> GetOrdinalDirections()
    {
        List<BlockAdjacency> directions = new();
        directions.Add(BlockAdjacency.NORTHEAST);
        directions.Add(BlockAdjacency.SOUTHEAST);
        directions.Add(BlockAdjacency.SOUTHWEST);
        directions.Add(BlockAdjacency.NORTHWEST);

        return directions;
    }

    public static BlockAdjacency GetOrdinalDirection(BlockAdjacency direction1, BlockAdjacency direction2)
    {
        if (direction1 == direction2)
        {
            Debug.LogError("Can not be same directions!");
            return direction1;
        }

        if (!cardinalDirections.Contains(direction1)
         || !cardinalDirections.Contains(direction2))
        {
            Debug.LogError("Must give 2 cardinal directions!");
            return direction1;
        }

        switch (direction1)
        {
            case BlockAdjacency.NORTH:
                if (direction2 == BlockAdjacency.WEST)
                    return BlockAdjacency.NORTHWEST;
                else if (direction2 == BlockAdjacency.EAST)
                    return BlockAdjacency.NORTHEAST;

                Debug.LogError("Directions are not orthogonal!");
                return direction1;

            case BlockAdjacency.SOUTH:
                if (direction2 == BlockAdjacency.WEST)
                    return BlockAdjacency.SOUTHWEST;
                else if (direction2 == BlockAdjacency.EAST)
                    return BlockAdjacency.SOUTHEAST;

                Debug.LogError("Directions are not orthogonal!");
                return direction1;

            case BlockAdjacency.WEST:
                if (direction2 == BlockAdjacency.NORTH)
                    return BlockAdjacency.NORTHWEST;
                else if (direction2 == BlockAdjacency.SOUTH)
                    return BlockAdjacency.SOUTHWEST;

                Debug.LogError("Directions are not orthogonal!");
                return direction1;

            case BlockAdjacency.EAST:
                if (direction2 == BlockAdjacency.NORTH)
                    return BlockAdjacency.NORTHEAST;
                else if (direction2 == BlockAdjacency.SOUTH)
                    return BlockAdjacency.SOUTHEAST;

                Debug.LogError("Directions are not orthogonal!");
                return direction1;
        }

        Debug.LogError("Incorrect directions given!");
        return direction1;
    }

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
        data.passable = true;
        data.buildable = true;

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

    public static bool IsBuildable(BlockData block)
    {
        return block.buildable
            && block.type != BlockType.WATER
            && block.type != BlockType.SNOW
            && block.type != BlockType.AIR;
    }
}
