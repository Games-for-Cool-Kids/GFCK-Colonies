using System;
using UnityEngine;

namespace World
{
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

    [Serializable]
    public class Block
    {
        public int x, y, z; // Local position in chunk.
        public Vector3 worldPosition;
        public BlockType type;
        public bool passable;
        public bool buildable;
    }
}
