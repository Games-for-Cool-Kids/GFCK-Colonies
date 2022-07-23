using UnityEngine;

public class ChunkGenerator
{
    private ChunkGeneratorStats _chunkStats;

    private ChunkData _generatedChunk = null;

    public volatile bool GenerationCompleted;

    public delegate void ChunkGenerationCallback(ChunkData chunk);
    ChunkGenerationCallback finishCallback;

    public ChunkGenerator(int x, int z, ChunkGeneratorStats stats, ChunkGenerationCallback generationCallback)
    {
        _chunkStats = stats;
        _generatedChunk = ChunkCode.CreateChunk(x, z, stats.origin, stats.chunkSize, stats.height);

        finishCallback = generationCallback;
    }

    public void Generate()
    {
        GenerateWorldChunk();
        FillHoles();

        GenerationCompleted = true;
    }

    public void NotifyCompleted()
    {
        finishCallback(_generatedChunk);
    }

    private void GenerateWorldChunk()
    {
        for (int x = 0; x < _chunkStats.chunkSize; x++)
        {
            for (int z = 0; z < _chunkStats.chunkSize; z++)
            {
                float height = _chunkStats.nodeGrid[x, z].height; // range 0.0 - 1.0
                int y = Mathf.FloorToInt(height * _chunkStats.height);

                Vector3 blockWorldPos = _chunkStats.origin + new Vector3(x, y, z);
                BlockData newBlock = BlockCode.CreateBlockData(x, y, z, true, _chunkStats.nodeGrid[x, z].type, blockWorldPos);
                ChunkCode.SetBlock(_generatedChunk, newBlock);
            }
        }
    }

    private void FillHoles()
    {
        foreach (BlockData surfaceBlock in ChunkCode.GetFilledBlocks(_generatedChunk))
        {
            var neighbors = ChunkCode.GetSurfaceNeighbors(_generatedChunk, surfaceBlock, false);

            int highestHeightDiff = 0;
            foreach (BlockData neighbor in neighbors)
            {
                int heightDiff = surfaceBlock.y - neighbor.y;
                if(heightDiff > highestHeightDiff)
                    highestHeightDiff = heightDiff;
            }

            int emptyBlocks = highestHeightDiff - 1;
            if (emptyBlocks > 0)
            {
                int x = surfaceBlock.x;
                int z = surfaceBlock.z;

                for (int y = surfaceBlock.y - 1; y >= surfaceBlock.y - emptyBlocks; y--)
                {
                    BlockData fill = BlockCode.CreateBlockData(x, y, z, true, BlockType.ROCK, new Vector3(x, y, z));
                    ChunkCode.SetBlock(_generatedChunk, fill);
                }
            }
        }
    }
}
