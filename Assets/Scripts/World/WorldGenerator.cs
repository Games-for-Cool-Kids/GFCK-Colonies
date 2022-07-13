using UnityEngine;
using SimplexNoise;

public class WorldGenerator
{
    public BlockGrid Grid;
    public WorldChunkStats ChunkStats;

    public delegate void WorldGenerationCallback(BlockGrid grid, WorldMeshData data);
    WorldGenerationCallback finishCallback;

    public void GenerateWorld(WorldChunkStats stats, WorldGenerationCallback generationCallback)
    {
        ChunkStats = stats;
        finishCallback = generationCallback;


        WorldMeshData worldData = CreateWorldData();
        CreateBlockMeshes(worldData);


        finishCallback(Grid, worldData);
    }

    private int GetNoise(int x, int y, int z, float scale, int max)
    {
        return Mathf.FloorToInt((Noise.Generate(x * scale, y * scale, z * scale) + 1) * (max / 2.0f));
    }

    private WorldMeshData CreateWorldData()
    {
        Grid = new BlockGrid(ChunkStats.MaxX, ChunkStats.MaxY, ChunkStats.MaxZ);

        WorldMeshData worldData = new WorldMeshData();
        for (int x = 0; x < ChunkStats.MaxX; x++)
        {
            for (int z = 0; z < ChunkStats.MaxZ; z++)
            {
                float height = GetNoise(x, 0, z, ChunkStats.Frequency, ChunkStats.MaxY);

                Block newBlock = new Block()
                {
                    x = x,
                    y = Mathf.RoundToInt(height),
                    z = z,
                    filled = true,
                };
                Grid.SetBlock(newBlock.x, newBlock.y, newBlock.z, newBlock);
            }
        }

        return worldData;
    }

    private void CreateBlockMeshes(WorldMeshData data)
    {
        foreach (Block filledBlock in Grid.GetFilledBlocks())
        {
            filledBlock.CreateMesh(data, Grid);
        }
    }
}
