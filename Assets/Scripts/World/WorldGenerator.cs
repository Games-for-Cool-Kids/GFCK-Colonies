using UnityEngine;
using SimplexNoise;

public class WorldGenerator
{
    private WorldChunkStats _chunkStats;
    private ChunkMeshData _chunkMeshData;

    private BlockGrid GeneratedGrid = null;

    public volatile bool GenerationCompleted;

    public delegate void WorldGenerationCallback(BlockGrid grid, ChunkMeshData data);
    WorldGenerationCallback finishCallback;

    public WorldGenerator(WorldChunkStats stats, WorldGenerationCallback generationCallback)
    {
        _chunkStats = stats;
        finishCallback = generationCallback;
    }

    public void GenerateWorld()
    {
        _chunkMeshData = CreateWorldChunk();
        CreateBlockMeshes();

        GenerationCompleted = true;
    }

    public void NotifyCompleted()
    {
        finishCallback(GeneratedGrid, _chunkMeshData);
    }

    private int GetNoise(float x, float y, float z, float scale, int max)
    {
        return Mathf.FloorToInt((Noise.Generate(x * scale, y * scale, z * scale) + 1) * (max / 2.0f));
    }

    private ChunkMeshData CreateWorldChunk()
    {
        GeneratedGrid = new BlockGrid(_chunkStats.chunkSize, _chunkStats.maxY, _chunkStats.chunkSize);

        ChunkMeshData worldData = new ChunkMeshData();
        worldData.origin = _chunkStats.origin;

        for (int x = 0; x < _chunkStats.chunkSize; x++)
        {
            for (int z = 0; z < _chunkStats.chunkSize; z++)
            {
                Vector3 noisePosition = _chunkStats.origin;
                noisePosition.x += x;
                noisePosition.z += z;

                // Noise is applied in y-axis only.
                int height = 0;
                if (_chunkStats.noisePatterns != null
                && _chunkStats.noisePatterns.Length > 0)
                {
                    for (int i = 0; i < _chunkStats.noisePatterns.Length; i++)
                    {
                        var noisePattern = _chunkStats.noisePatterns[i];
                        height += noisePattern.Calculate(_chunkStats, noisePosition);
                    }

                    // Clamp/Normalize height
                    height /= _chunkStats.noisePatterns.Length;
                    height = Mathf.Clamp(height, 0, _chunkStats.maxY - 1);
                }

                Block newBlock = new Block()
                {
                    x = x,
                    y = height,
                    z = z,
                    filled = true,
                };
                GeneratedGrid.SetBlock(newBlock.x, newBlock.y, newBlock.z, newBlock);
            }
        }

        return worldData;
    }

    private void CreateBlockMeshes()
    {
        foreach (Block filledBlock in GeneratedGrid.GetFilledBlocks())
        {
            filledBlock.CreateMesh(_chunkMeshData, GeneratedGrid);
        }
    }
}
