using UnityEngine;
using SimplexNoise;

public class WorldGenerator
{
    private WorldChunkStats ChunkStats;
    private WorldMeshData WorldMeshData;

    private BlockGrid GeneratedGrid = null;

    public volatile bool GenerationCompleted;

    public delegate void WorldGenerationCallback(BlockGrid grid, WorldMeshData data);
    WorldGenerationCallback finishCallback;

    public WorldGenerator(WorldChunkStats stats, WorldGenerationCallback generationCallback)
    {
        ChunkStats = stats;
        finishCallback = generationCallback;
    }

    public void GenerateWorld()
    {
        WorldMeshData = CreateWorldChunk();
        CreateBlockMeshes();

        GenerationCompleted = true;
    }

    public void NotifyCompleted()
    {
        finishCallback(GeneratedGrid, WorldMeshData);
    }

    private int GetNoise(float x, float y, float z, float scale, int max)
    {
        return Mathf.FloorToInt((Noise.Generate(x * scale, y * scale, z * scale) + 1) * (max / 2.0f));
    }

    private WorldMeshData CreateWorldChunk()
    {
        GeneratedGrid = new BlockGrid(ChunkStats.chunkSize, ChunkStats.maxY, ChunkStats.chunkSize);

        WorldMeshData worldData = new WorldMeshData();
        worldData.origin = ChunkStats.origin;

        for (int x = 0; x < ChunkStats.chunkSize; x++)
        {
            for (int z = 0; z < ChunkStats.chunkSize; z++)
            {
                Vector3 noisePosition = ChunkStats.origin;
                noisePosition.x += x;
                noisePosition.z += z;

                // Noise is applied in y-axis only.
                int height = 0;
                if (ChunkStats.noisePatterns != null)
                {
                    for (int i = 0; i < ChunkStats.noisePatterns.Length; i++)
                    {
                        var noisePattern = ChunkStats.noisePatterns[i];
                        height += noisePattern.Calculate(ChunkStats, noisePosition);
                    }

                    // Clamp/Normalize height
                    height /= ChunkStats.noisePatterns.Length;
                    height = Mathf.Clamp(height, 0, ChunkStats.maxY - 1);
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
            filledBlock.CreateMesh(WorldMeshData, GeneratedGrid);
        }
    }
}
