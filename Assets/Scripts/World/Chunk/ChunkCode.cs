using UnityEngine;
using System.Collections.Generic;

namespace World
{
    public static class ChunkExtensions
    {
        public static void CreateMeshData(this ChunkData chunk, GameWorldChunkData worldChunks)
        {
            chunk.meshData = new ChunkMeshData();
            chunk.meshChanged = true;

            foreach (Block filledBlock in chunk.GetFilledBlocks())
            {
                chunk.CreateBlockMesh(filledBlock, worldChunks);
            }
        }

        public static Mesh TakeMesh(this ChunkData chunk)
        {
            chunk.meshChanged = false;

            Mesh chunkMesh = new Mesh()
            {
                vertices = chunk.meshData.vertices.ToArray(),
                uv = chunk.meshData.uv.ToArray(),
                triangles = chunk.meshData.triangles.ToArray()
            };
            chunkMesh.RecalculateNormals();

            return chunkMesh;
        }

        public static void CreateBlockMesh(this ChunkData chunk, Block block, GameWorldChunkData worldChunks)
        {
            List<BlockAdjacency> sidesToCreate = new();
            foreach (var direction in BlockDirections.cardinalDirections)
            {
                if (!block.HasNeighbor(worldChunks, direction))
                    sidesToCreate.Add(direction);
            }

            /**
            //if (HasNeighbor(_chunks, dimensions, block.worldPosition, BlockAdjacency.ABOVE))
            //{
            //    ChunkMeshUtilities.CreateBlock(chunk.meshData, BlockExtensions.GetLocalPosition(block), sidesToCreate, block.type);
            //    return;
            //}
            //
            //if (sidesToCreate.Count == 1)
            //{
            //    if (HasNeighbor(_chunks, dimensions, block.worldPosition + Vector3.down, sidesToCreate[0]))
            //    {
            //        ChunkMeshUtilities.CreateSlopeBlock(chunk.meshData, BlockExtensions.GetLocalPosition(block), sidesToCreate[0], block.type);
            //        return;
            //    }
            //}
            //else if (sidesToCreate.Count == 2)
            //{
            //    BlockAdjacency ordinal = BlockExtensions.GetOrdinalDirection(sidesToCreate[0], sidesToCreate[1]);
            //    if (BlockExtensions.ordinalDirections.Contains(ordinal)
            //     && HasNeighbor(_chunks, dimensions, block.worldPosition + Vector3.down, sidesToCreate[0])
            //     && HasNeighbor(_chunks, dimensions, block.worldPosition + Vector3.down, sidesToCreate[1]))
            //    {
            //        ChunkMeshUtilities.CreateCornerSlopeBlock(chunk.meshData, BlockExtensions.GetLocalPosition(block), BlockExtensions.GetOrdinalDirection(sidesToCreate[0], sidesToCreate[1]), block.type);
            //        return;
            //    }
            //}
            **/

            if (!block.HasNeighbor(worldChunks, BlockAdjacency.ABOVE))
                sidesToCreate.Add(BlockAdjacency.ABOVE);

            ChunkMeshUtilities.CreateBlock(chunk.meshData, block.GetLocalPosition(), sidesToCreate, block.type);
        }

        public static void SetBlock(this ChunkData chunk, Block block)
        {
            chunk.blocks[block.x, block.y, block.z] = block;
        }

        public static Vector3 GetLocalPos(this ChunkData chunk, Vector3 worldPos)
        {
            Vector3 localPos = worldPos - chunk.origin;
            int x = Mathf.FloorToInt(localPos.x);
            int y = Mathf.FloorToInt(localPos.y);
            int z = Mathf.FloorToInt(localPos.z);

            return new Vector3(x, y, z);
        }

        private static bool InBounds(this ChunkData chunk, int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= 0
            && x < chunk.MaxX && y < chunk.MaxY && z < chunk.MaxZ;
        }

        public static Block GetBlock(this ChunkData chunk, int x, int y, int z)
        {
            if (!chunk.InBounds(x, y, z))
                return null;

            return chunk.blocks[x, y, z];
        }

        public static Block GetBlockAt(this ChunkData chunk, Vector3 worldPos)
        {
            worldPos -= chunk.origin; // Make relative to chunk.

            int blockX = Mathf.RoundToInt(worldPos.x);
            int blockY = Mathf.RoundToInt(worldPos.y);
            int blockZ = Mathf.RoundToInt(worldPos.z);

            return chunk.GetBlock(blockX, blockY, blockZ);
        }

        public static Block GetSurfaceBlock(this ChunkData chunk, int x, int z)
        {
            if (x < 0 || z < 0
            || x >= chunk.MaxX || z >= chunk.MaxZ)
                return null;

            for (int y = chunk.MaxY - 1; y > 0; y--) // Search from top-down until we hit a surface block.
            {
                Block block = chunk.GetBlock(x, y, z);
                if (block.IsSolidBlock())
                    return block;
            }

            return null;
        }

        public static Block GetBlockAdjacentTo(this ChunkData chunk, Block sourceBlock, BlockAdjacency adjacency)
        {
            int x = sourceBlock.x;
            int y = sourceBlock.y;
            int z = sourceBlock.z;

            switch (adjacency)
            {
                case BlockAdjacency.NORTH:
                    z = z + 1; break;
                case BlockAdjacency.SOUTH:
                    z = z - 1; break;
                case BlockAdjacency.WEST:
                    x = x - 1; break;
                case BlockAdjacency.EAST:
                    x = x + 1; break;
                case BlockAdjacency.ABOVE:
                    y = y + 1; break;
                case BlockAdjacency.BELOW:
                    y = y - 1; break;
            }

            return chunk.GetBlock(x, y, z);
        }

        public static List<Block> GetSurfaceNeighbors(this ChunkData chunk, int x, int y, int z, bool diagonal = true)
        {
            Block block = chunk.GetBlock(x, y, z);
            if (block != null)
                return chunk.GetNeighborSurfaceBlocks(block);

            return new List<Block>(); // empty list
        }

        public static List<Block> GetNeighborSurfaceBlocks(this ChunkData chunk, Block source, bool diagonal = true)
        {
            List<Block> neighbors = new List<Block>();

            for (int x = source.x - 1; x <= source.x + 1; x++)
            {
                for (int z = source.z - 1; z <= source.z + 1; z++)
                {
                    if (x == source.x && z == source.z) // Skip self.
                        continue;

                    for (int y = chunk.MaxY - 1; y > 0; y--) // Search from top-down until we hit a surface block.
                    {
                        if (!diagonal // Skip diagonal blocks.
                         && Mathf.Abs(x - source.x) == 1
                         && Mathf.Abs(z - source.z) == 1)
                            continue;

                        Block neighbor = chunk.GetBlock(x, y, z);
                        if (neighbor.IsSolidBlock())
                        {
                            neighbors.Add(neighbor);
                            y = 0; // Stop top-down search.
                        }
                    }
                }
            }

            return neighbors;
        }

        public static List<Block> GetFilledBlocks(this ChunkData chunk)
        {
            List<Block> filledBlocks = new();

            for (int x = 0; x < chunk.MaxX; x++)
            {
                for (int y = 0; y < chunk.MaxY; y++)
                {
                    for (int z = 0; z < chunk.MaxZ; z++)
                    {
                        Block block = chunk.blocks[x, y, z];
                        if (block.IsSolidBlock())
                            filledBlocks.Add(chunk.GetBlock(x, y, z));
                    }
                }
            }

            return filledBlocks;
        }

        public static List<Block> GetWalkableBlocks(this ChunkData chunk)
        {
            List<Block> walkableBlocks = new();

            for (int x = 0; x < chunk.MaxX; x++)
            {
                for (int y = 0; y < chunk.MaxY; y++)
                {
                    for (int z = 0; z < chunk.MaxZ; z++)
                    {
                        Block block = chunk.blocks[x, y, z];
                        if (block != null
                         && block.IsWalkable(chunk))
                            walkableBlocks.Add(chunk.GetBlock(x, y, z));
                    }
                }
            }

            return walkableBlocks;
        }

        public static void CreateBlocksUnder(this ChunkData chunk, Block block, int amount, BlockType type = BlockType.ROCK)
        {
            int x = block.x;
            int z = block.z;

            for (int y = block.y - 1; y >= block.y - amount; y--)
            {
                Block newBlock = BlockFactory.CreateBlock(x, y, z,
                                           type,
                                           new Vector3(block.worldPosition.x, y, block.worldPosition.z));
                chunk.SetBlock(newBlock);
            }
        }

        public static void AddBlock(this ChunkData chunk, Block newBlock, GameWorldChunkData worldChunks)
        {
            if (!chunk.InBounds(newBlock.x, newBlock.y, newBlock.z))
                return;

            chunk.blocks[newBlock.x, newBlock.y, newBlock.z] = newBlock;

            chunk.CreateMeshData(worldChunks); // Update mesh.
        }

        public static void RemoveBlock(this ChunkData chunk, Block block, GameWorldChunkData worldChunks)
        {
            if (block.y <= 0) // Cannot destroy bottom-most block.
                return;

            int x = block.x;
            int y = block.y;
            int z = block.z;

            chunk.blocks[x, y, z].type = BlockType.AIR; // Set block to empty.

            chunk.CreateMeshData(worldChunks); // Update mesh.
        }
    }
}