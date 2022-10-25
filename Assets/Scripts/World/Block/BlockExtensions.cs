using UnityEngine;

namespace World
{
    public static class BlockExtensions
    {
        public static Vector3 GetLocalPosition(this BlockData block)
        {
            return new Vector3(block.x, block.y, block.z);
        }

        public static Vector3 GetSurfaceWorldPos(this BlockData block)
        {
            return block.worldPosition + Vector3.up / 2;
        }

        public static bool IsBuildable(this BlockData block)
        {
            return block != null
                && block.buildable
                && block.type != BlockType.WATER
                && block.type != BlockType.SNOW
                && block.type != BlockType.AIR;
        }
    }
}
