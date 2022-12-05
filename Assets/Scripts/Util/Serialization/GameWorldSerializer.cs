using UnityEngine;
using System;
using System.Collections.Generic;
using World;
using static Unity.VisualScripting.Member;
using static UnityEditor.Rendering.CameraUI;

namespace Serialization
{
    [Serializable]
    struct BlockGridPackage
    {
        public List<Block> SerializableBlocks;
    }

    [Serializable]
    internal class GameWorldSerializer
    {
        private GameWorld _world;

        public List<TwoDimensionalPackage<Chunk>> SerializableChunks;
        public List<BlockGridPackage> SerializableBlockGrids;
        public List<TwoDimensionalPackage<GameObject>> SerializableChunkObjects;


        public void Serialize(GameWorld world)
        {
            if (world.worldChunks.chunks == null
            || world.chunkObjects == null)
                return;

            _world = world;

            Serializers.SerializeTwoDimensional(_world.worldChunks.chunks, out SerializableChunks);
            Serializers.SerializeTwoDimensional(_world.chunkObjects, out SerializableChunkObjects);

            SerializableBlockGrids = new();
            foreach (var chunk_package in SerializableChunks)
            {
                SerializeBlockGrid(chunk_package.Element, out BlockGridPackage blockGridPackage);
                SerializableBlockGrids.Add(blockGridPackage);
            }
        }

        public void Deserialize()
        {
            if (_world == null)
                return;
            Debug.Assert(SerializableChunks.Count == SerializableBlockGrids.Count);

            int dimension = _world.worldChunks.worldChunkWidth;

            // Chunk objects
            Serializers.DeserializeTwoDimensional(SerializableChunkObjects, dimension, dimension, out _world.chunkObjects);

            // Chunks and their block grids
            _world.worldChunks.chunks = new Chunk[dimension, dimension];
            for (int i = 0; i < SerializableChunks.Count; i++)
            {
                var chunk_package = SerializableChunks[i];
                var chunk = chunk_package.Element;

                _world.worldChunks.chunks[chunk_package.Index0, chunk_package.Index1] = chunk;

                DeserializeBlockGrid(SerializableBlockGrids[i], ref chunk);
            }
        }

        private void SerializeBlockGrid(Chunk chunk, out BlockGridPackage blockGridPackage)
        {
            blockGridPackage = new()
            { 
                SerializableBlocks = new()
            };

            for (int x = 0; x < chunk.MaxX; x++)
                for (int y = 0; y < chunk.MaxY; y++)
                    for (int z = 0; z < chunk.MaxZ; z++)
                    {
                        Block block = chunk.blocks[x, y, z];
                        if (block != null)
                            blockGridPackage.SerializableBlocks.Add(block);
                    }
        }

        private void DeserializeBlockGrid(BlockGridPackage blockGridPackage, ref Chunk chunk)
        {
            chunk.blocks = new Block[chunk.MaxX, chunk.MaxY, chunk.MaxZ];
            foreach(var block in blockGridPackage.SerializableBlocks)
            {
                chunk.blocks[block.x, block.y, block.z] = block;
            }
        }
    }
}
