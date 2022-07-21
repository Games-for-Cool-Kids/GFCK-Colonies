using UnityEngine;
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

    // Status
    private bool chunksGenerated = false;
    public volatile bool worldGenCompleted = false;

    // Output
    private Chunk[,] chunks;

    // Finished callback
    public delegate void WorldGenerationFinishedCallBack(Chunk[,] chunks);
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

        CreateChunks();
    }

    public void Run()
    {
        if (worldGenCompleted)
            return;

        if (!chunksGenerated)
            UpdateChunkGeneratorThreads();
        else
        {
            FillHoles();
            CreateChunkMeshes();
            worldGenCompleted = true;
        }
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
            chunksGenerated = true;
    }

    public void NotifyCompleted()
    {
        worldGenFinishedCallback(chunks);
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

    private void FillHoles()
    {
        for (int c_x = 0; c_x < worldChunkWidth; c_x++)
        {
            for (int c_z = 0; c_z < worldChunkWidth; c_z++)
            {
                FillNeighboringEdge(c_x, c_z, BlockGrid.Adjacency.NORTH);
                FillNeighboringEdge(c_x, c_z, BlockGrid.Adjacency.SOUTH);
                FillNeighboringEdge(c_x, c_z, BlockGrid.Adjacency.EAST);
                FillNeighboringEdge(c_x, c_z, BlockGrid.Adjacency.WEST);
            }
        }
    }

    private void FillNeighboringEdge(int x, int z, BlockGrid.Adjacency direction)
    {
        Chunk current = chunks[x, z];
        Chunk neighbor = null;
        switch (direction)
        {
            case BlockGrid.Adjacency.NORTH:
                neighbor = GetChunk(x, z + 1);
                break;
            case BlockGrid.Adjacency.SOUTH:
                neighbor = GetChunk(x, z - 1);
                break;
            case BlockGrid.Adjacency.EAST:
                neighbor = GetChunk(x + 1, z);
                break;
            case BlockGrid.Adjacency.WEST:
                neighbor = GetChunk(x - 1, z);
                break;
        }
        if (neighbor == null)
            return;


        for (int i = 0; i < chunkSize; i++)
        {
            Block currentBlock = null;
            Block neighborBlock = null;
            switch(direction)
            {
                case BlockGrid.Adjacency.NORTH:
                    currentBlock = current.grid.GetSurfaceBlock(i, chunkSize - 1);
                    neighborBlock = neighbor.grid.GetSurfaceBlock(i, 0);
                    break;
                case BlockGrid.Adjacency.SOUTH:
                    currentBlock = current.grid.GetSurfaceBlock(i, 0);
                    neighborBlock = neighbor.grid.GetSurfaceBlock(i, chunkSize - 1);
                    break;
                case BlockGrid.Adjacency.EAST:
                    currentBlock = current.grid.GetSurfaceBlock(chunkSize - 1, i);
                    neighborBlock = neighbor.grid.GetSurfaceBlock(0, i);
                    break;
                case BlockGrid.Adjacency.WEST:
                    currentBlock = current.grid.GetSurfaceBlock(0, i);
                    neighborBlock = neighbor.grid.GetSurfaceBlock(chunkSize - 1, i);
                    break;
            }

            int blocksToFill = currentBlock.y - neighborBlock.y - 1;
            if (blocksToFill > 0)
            {
                for (int y = currentBlock.y - 1; y >= currentBlock.y - blocksToFill; y--)
                {
                    Block fill = new Block(currentBlock.x, y, currentBlock.z, true, Block.Type.ROCK, new Vector3(currentBlock.x, y, currentBlock.z));
                    current.grid.SetBlock(currentBlock.x, y, currentBlock.z, fill);
                }
            }
        }
    }

    private void CreateChunkMeshes()
    {
        foreach(var chunk in chunks)
        {
            foreach (Block filledBlock in chunk.grid.GetFilledBlocks())
            {
                filledBlock.CreateMesh(chunk.meshData, chunk.grid);
            }
        }
    }

    public Chunk GetChunk(int x, int z)
    {
        if (x < 0 || x >= worldChunkWidth
        || z < 0 || z >= worldChunkWidth)
            return null;

        return chunks[x, z];
    }
}
