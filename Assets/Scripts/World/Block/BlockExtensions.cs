using UnityEngine;

namespace World.Block
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
    }
}
