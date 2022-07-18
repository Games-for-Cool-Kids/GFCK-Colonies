using UnityEngine;

public class ChunkGenerator
{
    private ChunkGeneratorStats _chunkStats;

    private Chunk _generatedChunk = null;

    public volatile bool GenerationCompleted;

    public delegate void ChunkGenerationCallback(Chunk chunk);
    ChunkGenerationCallback finishCallback;

    public ChunkGenerator(int x, int z, ChunkGeneratorStats stats, ChunkGenerationCallback generationCallback)
    {
        _chunkStats = stats;
        _generatedChunk = new Chunk(x, z, stats.origin, stats.chunkSize, stats.height);

        finishCallback = generationCallback;
    }

    public void Generate()
    {
        GenerateWorldChunk();
        CreateBlockMeshes();

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
                int y = _chunkStats.heightMap[x, z];

                Vector3 blockWorldPos = _chunkStats.origin + new Vector3(x, y, z);
                Block newBlock = new Block(x, y, z, true, _chunkStats.blockMap[x, z], blockWorldPos);
                _generatedChunk.grid.SetBlock(newBlock.x, newBlock.y, newBlock.z, newBlock);
            }
        }
    }

    private void CreateBlockMeshes()
    {
        foreach (Block filledBlock in _generatedChunk.grid.GetFilledBlocks())
        {
            filledBlock.CreateMesh(_generatedChunk.meshData, _generatedChunk.grid);
        }
    }
}
