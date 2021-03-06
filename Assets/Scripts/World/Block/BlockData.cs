using UnityEngine;

public enum BlockType
{
    AIR,
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
    public Vector3 worldPosition;
    public BlockType type;
}
