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
        float singleBlockHeight = 1 / _chunkStats.height;
        for (int x = 0; x < _chunkStats.chunkSize; x++)
        {
            for (int z = 0; z < _chunkStats.chunkSize; z++)
            {
                float height = _chunkStats.nodeGrid[x, z].height; // range 0.0 - 1.0
                float rescaledHeight = height / (1 - singleBlockHeight); // Rescale is needed because default height range is inclusive for both 0 and 1. But because 'y' is a y-index it has to exclude the max height.
                int y = Mathf.FloorToInt(height * _chunkStats.height);

                Vector3 blockWorldPos = _chunkStats.origin + new Vector3(x, y, z);
                Block newBlock = new Block(x, y, z, true, _chunkStats.nodeGrid[x, z].type, blockWorldPos);
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
