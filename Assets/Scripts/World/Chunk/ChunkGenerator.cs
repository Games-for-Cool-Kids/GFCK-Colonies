using UnityEngine;
using SimplexNoise;

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
        _generatedChunk = new Chunk(x, z, stats.origin, stats.chunkSize, stats.maxY);

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

    private int GetNoise(float x, float y, float z, float scale, int max)
    {
        return Mathf.FloorToInt((Noise.Generate(x * scale, y * scale, z * scale) + 1) * (max / 2.0f));
    }

    private void GenerateWorldChunk()
    {
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
