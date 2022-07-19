using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class WorldGenerator
{
    private int maxWorkers = 8;
    private List<ChunkGenerator> toDoWorkers = new List<ChunkGenerator>();
    private List<ChunkGenerator> currentWorkers = new List<ChunkGenerator>();

    private int chunkSize;
    private int worldChunkWidth;

    private WorldVariable worldVariable;

    // Output
    private Chunk[,] chunks;
    private BlockGrid worldGrid;

    // Finished callback
    public volatile bool worldGenCompleted;
    public delegate void WorldGenerationFinishedCallBack(Chunk[,] chunks, BlockGrid worldGrid);
    private WorldGenerationFinishedCallBack worldGenFinishedCallback;

    public WorldGenerator(int chunkSize,
                          int worldChunkWidth,
                          WorldVariable worldVariable,
                          WorldGenerationFinishedCallBack worldGenerationFinishedCallback)
    {
        this.chunkSize = chunkSize;
        this.worldChunkWidth = worldChunkWidth;
        this.worldVariable = worldVariable;
        this.worldGenFinishedCallback = worldGenerationFinishedCallback;

        worldGrid = new(worldVariable.size, worldVariable.height, worldVariable.size);

        CreateChunks();
    }

    public void Run()
    {
        UpdateChunkGeneratorThreads();
    }

    private void UpdateChunkGeneratorThreads()
    {
        int i = 0;
        while (i < currentWorkers.Count)
        {
            if (currentWorkers[i].GenerationCompleted)
            {
                currentWorkers[i].NotifyCompleted();
                currentWorkers.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        if (toDoWorkers.Count > 0
        && currentWorkers.Count < maxWorkers)
        {
            ChunkGenerator generator = toDoWorkers[0];
            toDoWorkers.RemoveAt(0);
            currentWorkers.Add(generator);

            Thread workerThread = new Thread(generator.Generate);
            workerThread.Start();
        }

        if (currentWorkers.Count == 0)
            worldGenCompleted = true;
    }

    public void NotifyCompleted()
    {
        worldGenFinishedCallback(chunks, worldGrid);
    }

    private void CreateChunks()
    {
        chunks = new Chunk[worldChunkWidth, worldChunkWidth];

        for (int x = 0; x < worldChunkWidth; x++)
        {
            for (int z = 0; z < worldChunkWidth; z++)
            {
                Vector3 chunkPos = Vector3.zero;
                chunkPos.x += x * chunkSize;
                chunkPos.z += z * chunkSize;

                RequestWorldChunkGeneration(x, z, chunkPos);
            }
        }
    }

    public void RequestWorldChunkGeneration(int x, int z, Vector3 position)
    {
        ChunkGenerator generator = new(x, z, CreateChunkStats(x, z, position), AddChunk);
        toDoWorkers.Add(generator);
    }

    private ChunkGeneratorStats CreateChunkStats(int x, int z, Vector3 position)
    {
        return new ChunkGeneratorStats
        {
            chunkSize = this.chunkSize,
            height = worldVariable.height,
            origin = position,
            nodeGrid = worldVariable.GetChunkNodeGrid(x, z, chunkSize),
        };
    }

    private void AddChunk(Chunk chunk)
    {
        chunks[chunk.x, chunk.z] = chunk; // Store generated chunk
    }
}
