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
        WorldMeshData = CreateWorldData();
        CreateBlockMeshes();

        GenerationCompleted = true;
    }

    public void NotifyCompleted()
    {
        finishCallback(GeneratedGrid, WorldMeshData);
    }

    private int GetNoise(int x, int y, int z, float scale, int max)
    {
        return Mathf.FloorToInt((Noise.Generate(x * scale, y * scale, z * scale) + 1) * (max / 2.0f));
    }

    private WorldMeshData CreateWorldData()
    {
        GeneratedGrid = new BlockGrid(ChunkStats.maxX, ChunkStats.maxY, ChunkStats.maxZ);

        WorldMeshData worldData = new WorldMeshData();
        for (int x = 0; x < ChunkStats.maxX; x++)
        {
            for (int z = 0; z < ChunkStats.maxZ; z++)
            {
                float height = GetNoise(x, 0, z, ChunkStats.frequency, ChunkStats.maxY);

                Block newBlock = new Block()
                {
                    x = x,
                    y = Mathf.FloorToInt(height),
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
