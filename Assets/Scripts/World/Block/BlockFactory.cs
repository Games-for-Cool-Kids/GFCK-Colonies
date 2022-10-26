using UnityEngine;

namespace World
{
    internal class BlockFactory
    {
        public static BlockData CreateBlock(Vector3 localPosition, BlockType type, Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(localPosition.x);
            int y = Mathf.FloorToInt(localPosition.y);
            int z = Mathf.FloorToInt(localPosition.z);

            return CreateBlock(x, y, z, type, worldPosition);
        }

        public static BlockData CreateBlock(int x, int y, int z, BlockType type, Vector3 worldPosition)
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
    }
}
