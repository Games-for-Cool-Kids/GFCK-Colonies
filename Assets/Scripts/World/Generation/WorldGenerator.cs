using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Threading;
using World;


public class WorldGenerator
{
    private int _maxWorkers = 8;
    private List<ChunkGenerator> _toDoWorkers = new List<ChunkGenerator>();
    private List<ChunkGenerator> _currentWorkers = new List<ChunkGenerator>();

    private int _chunkSize;
    private int _worldChunkWidth;

    private Material _material;

    private WorldVariable worldVariable;

    private Marching _marching = new MarchingCubes();

    private VoxelArray _voxels;

    // Status
    private bool _chunksGenerated = false;
    public volatile bool worldGenCompleted = false;

    // Output
    private ChunkData[,] _chunks;

    // Finished callback
    public delegate void WorldGenerationFinishedCallBack(ChunkData[,] chunks);
    private WorldGenerationFinishedCallBack _worldGenFinishedCallback;


    public WorldGenerator(int chunkSize,
                          int worldChunkWidth,
                          Material material,
                          WorldVariable worldVariable,
                          WorldGenerationFinishedCallBack worldGenerationFinishedCallback)
    {
        this._chunkSize = chunkSize;
        this._worldChunkWidth = worldChunkWidth;
        this.worldVariable = worldVariable;
        _material = material;
        this._worldGenFinishedCallback = worldGenerationFinishedCallback;
        this._voxels = new VoxelArray(chunkSize * worldChunkWidth, worldVariable.height + 1, chunkSize * worldChunkWidth);

        CreateChunks();
    }

    public void Run()
    {
        Debug.Assert(worldGenCompleted == false);

        if (!_chunksGenerated)
            UpdateChunkGeneratorThreads();
        else
        {
            FillHoles();
            CreateChunkMeshes();
            //GenerateVoxelMesh();

            worldGenCompleted = true;
        }
    }

    private void UpdateChunkGeneratorThreads()
    {
        int i = 0;
        while (i < _currentWorkers.Count)
        {
            if (_currentWorkers[i].GenerationCompleted)
            {
                _currentWorkers[i].NotifyCompleted();
                _currentWorkers.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        if (_toDoWorkers.Count > 0
        && _currentWorkers.Count < _maxWorkers)
        {
            ChunkGenerator generator = _toDoWorkers[0];
            _toDoWorkers.RemoveAt(0);
            _currentWorkers.Add(generator);

            Thread workerThread = new Thread(generator.Generate);
            workerThread.Start();
        }

        if (_currentWorkers.Count == 0)
            _chunksGenerated = true;
    }

    public void NotifyCompleted()
    {
        _worldGenFinishedCallback(_chunks);
    }

    private void CreateChunks()
    {
        _chunks = new ChunkData[_worldChunkWidth, _worldChunkWidth];

        for (int x = 0; x < _worldChunkWidth; x++)
        {
            for (int z = 0; z < _worldChunkWidth; z++)
            {
                Vector3 chunkPos = Vector3.zero;
                chunkPos.x += x * _chunkSize;
                chunkPos.z += z * _chunkSize;

                RequestWorldChunkGeneration(x, z, chunkPos);
            }
        }
    }

    public void RequestWorldChunkGeneration(int x, int z, Vector3 position)
    {
        ChunkGenerator generator = new(x, z, CreateChunkStats(x, z, position), AddChunk);
        _toDoWorkers.Add(generator);
    }

    private ChunkGeneratorStats CreateChunkStats(int x, int z, Vector3 position)
    {
        return new ChunkGeneratorStats
        {
            chunkSize = this._chunkSize,
            height = worldVariable.height,
            origin = position,
            nodeGrid = worldVariable.GetChunkNodeGrid(x, z, _chunkSize),
        };
    }

    private void AddChunk(ChunkData chunk)
    {
        _chunks[chunk.x, chunk.z] = chunk; // Store generated chunk
    }

    private void FillHoles()
    {
        for (int c_x = 0; c_x < _worldChunkWidth; c_x++)
        {
            for (int c_z = 0; c_z < _worldChunkWidth; c_z++)
            {
                ChunkCode.FillNeighboringEdge(_chunks, GetWorldChunkDimensions(), c_x, c_z, BlockAdjacency.NORTH);
                ChunkCode.FillNeighboringEdge(_chunks, GetWorldChunkDimensions(), c_x, c_z, BlockAdjacency.SOUTH);
                ChunkCode.FillNeighboringEdge(_chunks, GetWorldChunkDimensions(), c_x, c_z, BlockAdjacency.EAST);
                ChunkCode.FillNeighboringEdge(_chunks, GetWorldChunkDimensions(), c_x, c_z, BlockAdjacency.WEST);
            }
        }
    }

    private void CreateChunkMeshes()
    {
        foreach(var chunk in _chunks)
        {
            ChunkCode.CreateMeshData(_chunks, GetWorldChunkDimensions(), chunk);
        }
    }

    private void GenerateVoxelMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        _marching.Generate(_voxels.Voxels, verts, indices);

        var position = new Vector3(-_worldChunkWidth / 2, -_worldChunkWidth / 2, -_chunkSize / 2);

        CreateMesh32(verts, normals, indices, uvs, position);
    }

    private GameWorld.ChunkDimensions GetWorldChunkDimensions()
    {
        return new GameWorld.ChunkDimensions { chunkSize = this._chunkSize, worldChunkWidth = this._worldChunkWidth };
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
