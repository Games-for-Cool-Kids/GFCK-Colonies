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
