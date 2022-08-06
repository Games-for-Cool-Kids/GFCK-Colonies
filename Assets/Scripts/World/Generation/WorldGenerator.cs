using UnityEngine;
using UnityEngine.Rendering;
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

    private Material _material;

    private WorldVariable worldVariable;

    //private Marching _marching = new MarchingTertrahedron();
    private Marching _marching = new MarchingCubes();

    private VoxelArray _voxels;

    // Status
    private bool chunksGenerated = false;
    public volatile bool worldGenCompleted = false;

    // Output
    private ChunkData[,] chunks;

    // Finished callback
    public delegate void WorldGenerationFinishedCallBack(ChunkData[,] chunks);
    private WorldGenerationFinishedCallBack worldGenFinishedCallback;


    public WorldGenerator(int chunkSize,
                          int worldChunkWidth,
                          Material material,
                          WorldVariable worldVariable,
                          WorldGenerationFinishedCallBack worldGenerationFinishedCallback)
    {
        this.chunkSize = chunkSize;
        this.worldChunkWidth = worldChunkWidth;
        this.worldVariable = worldVariable;
        _material = material;
        this.worldGenFinishedCallback = worldGenerationFinishedCallback;
        this._voxels = new VoxelArray(chunkSize * worldChunkWidth, worldVariable.height + 1, chunkSize * worldChunkWidth);

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
            SetFilledVoxels();
            //GenerateVoxelMesh();
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
        chunks = new ChunkData[worldChunkWidth, worldChunkWidth];

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

    private void AddChunk(ChunkData chunk)
    {
        chunks[chunk.x, chunk.z] = chunk; // Store generated chunk
    }

    private void FillHoles()
    {
        for (int c_x = 0; c_x < worldChunkWidth; c_x++)
        {
            for (int c_z = 0; c_z < worldChunkWidth; c_z++)
            {
                ChunkCode.FillNeighboringEdge(chunks, GetWorldChunkDimensions(), c_x, c_z, BlockAdjacency.NORTH);
                ChunkCode.FillNeighboringEdge(chunks, GetWorldChunkDimensions(), c_x, c_z, BlockAdjacency.SOUTH);
                ChunkCode.FillNeighboringEdge(chunks, GetWorldChunkDimensions(), c_x, c_z, BlockAdjacency.EAST);
                ChunkCode.FillNeighboringEdge(chunks, GetWorldChunkDimensions(), c_x, c_z, BlockAdjacency.WEST);
            }
        }
    }

    private void SetFilledVoxels()
    {
        foreach(var chunk in chunks)
        {
            ChunkCode.CreateMeshData(chunks, GetWorldChunkDimensions(), chunk);
        }
    }

    private void GenerateVoxelMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        _marching.Generate(_voxels.Voxels, verts, indices);

        var position = new Vector3(-worldChunkWidth / 2, -worldChunkWidth / 2, -chunkSize / 2);

        CreateMesh32(verts, normals, indices, uvs, position);
    }

    private World.ChunkDimensions GetWorldChunkDimensions()
    {
        return new World.ChunkDimensions { chunkSize = this.chunkSize, worldChunkWidth = this.worldChunkWidth };
    }

    private void CreateMesh32(List<Vector3> verts, List<Vector3> normals, List<int> indices, List<Vector2> uvs,  Vector3 position)
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(verts);
        mesh.SetTriangles(indices, 0);
        mesh.SetUVs(0, uvs);

        if (normals.Count > 0)
            mesh.SetNormals(normals);
        else
            mesh.RecalculateNormals();

        mesh.RecalculateBounds();

        GameObject go = new GameObject("TerrainMesh");
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>().material = _material;
        go.transform.localPosition = position;
    }
}
