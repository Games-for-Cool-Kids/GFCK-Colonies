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

        public static bool IsSolidBlock(this BlockData block)
        {
            return block != null
                && block.type != BlockType.AIR;
        }

        public static bool IsBuildable(this BlockData block)
        {
            return block != null
                && block.buildable
                && block.type != BlockType.WATER
                && block.type != BlockType.SNOW
                && block.type != BlockType.AIR;
        }

        public static bool HasNeighbor(this BlockData block, GameWorldChunkData worldChunks, BlockAdjacency direction)
        {
            Vector3 offset = Vector3.zero;
            switch (direction)
            {
                case BlockAdjacency.NORTH:
                    offset = Vector3.forward;
                    break;
                case BlockAdjacency.SOUTH:
                    offset = Vector3.back;
                    break;
                case BlockAdjacency.WEST:
                    offset = Vector3.left;
                    break;
                case BlockAdjacency.EAST:
                    offset = Vector3.right;
                    break;
                case BlockAdjacency.ABOVE:
                    offset = Vector3.up;
                    break;
                case BlockAdjacency.BELOW:
                    offset = Vector3.down;
                    break;
            }

            var neighbor = worldChunks.GetBlockAt(block.worldPosition + offset);
            return neighbor.IsSolidBlock();
        }

        public static bool IsSurfaceBlock(this BlockData block, ChunkData chunk)
        {
            BlockData blockAbove = chunk.GetBlockAdjacentTo(block, BlockAdjacency.ABOVE);
            return blockAbove == null
                || blockAbove.type == BlockType.AIR;
        }

        public static bool IsWalkable(this BlockData block, ChunkData chunk)
        {
            return block.IsSurfaceBlock(chunk)
                && block.type != BlockType.AIR
                && block.type != BlockType.WATER
                && block.passable; // Determines if it can be used for pathfinding.
        }
    }
}
