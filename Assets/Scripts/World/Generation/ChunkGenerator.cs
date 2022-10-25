using UnityEngine;
using World;

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
                BlockData newBlock = BlockFactory.CreateBlock(x, y, z, _chunkStats.nodeGrid[x, z].type, blockWorldPos);
                ChunkCode.SetBlock(_generatedChunk, newBlock);

                ChunkCode.CreateBlocksUnder(_generatedChunk, newBlock, y);
            }
        }
    }
}
