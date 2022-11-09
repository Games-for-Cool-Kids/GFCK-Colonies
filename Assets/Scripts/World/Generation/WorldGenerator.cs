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

    private Material _material;

    private WorldVariable _worldVariable;

    private Marching _marching = new MarchingCubes();

    private VoxelArray _voxels;

    // Status
    private bool _chunksGenerated = false;
    public volatile bool worldGenCompleted = false;

    // Output
    private GameWorldChunkData _worldChunks = new();

    // Finished callback
    public delegate void WorldGenerationFinishedCallBack(Chunk[,] chunks);
    private WorldGenerationFinishedCallBack _worldGenFinishedCallback;

    // Fields
    private int _chunkSize
    { get => _worldChunks.chunkSize; }
    private int _worldChunkWidth
    { get => _worldChunks.worldChunkWidth; }


    public WorldGenerator(Material material,
                          WorldVariable worldVariable,
                          WorldGenerationFinishedCallBack worldGenerationFinishedCallback)
    {
        this._worldChunks.worldChunkWidth = Mathf.FloorToInt(worldVariable.size / _worldChunks.chunkSize);
        this._worldChunks.blockHeight = worldVariable.height;
        this._worldVariable = worldVariable;
        this._material = material;
        this._worldGenFinishedCallback = worldGenerationFinishedCallback;
        this._voxels = new VoxelArray(_worldChunks.chunkSize * _worldChunks.worldChunkWidth, worldVariable.height + 1, _worldChunks.chunkSize * _worldChunks.worldChunkWidth);

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
        _worldGenFinishedCallback(_worldChunks.chunks);
    }

    private void CreateChunks()
    {
        _worldChunks.chunks = new Chunk[_worldChunkWidth, _worldChunkWidth];

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
            chunkSize = _chunkSize,
            height = _worldVariable.height,
            origin = position,
            nodeGrid = _worldVariable.GetChunkNodeGrid(x, z, _chunkSize),
        };
    }

    private void AddChunk(Chunk chunk)
    {
        _worldChunks.chunks[chunk.x, chunk.z] = chunk; // Store generated chunk
    }

    private void FillHoles()
    {
        for (int c_x = 0; c_x < _worldChunkWidth; c_x++)
        {
            for (int c_z = 0; c_z < _worldChunkWidth; c_z++)
            {
                FillNeighboringEdge(c_x, c_z, BlockAdjacency.NORTH);
                FillNeighboringEdge(c_x, c_z, BlockAdjacency.SOUTH);
                FillNeighboringEdge(c_x, c_z, BlockAdjacency.EAST);
                FillNeighboringEdge(c_x, c_z, BlockAdjacency.WEST);
            }
        }
    }

    public void FillNeighboringEdge(int x, int z, BlockAdjacency direction)
    {
        Chunk current = _worldChunks.chunks[x, z];
        Chunk neighbor = null;
        switch (direction)
        {
            case BlockAdjacency.NORTH:
                neighbor = _worldChunks.GetChunk(x, z + 1);
                break;
            case BlockAdjacency.SOUTH:
                neighbor = _worldChunks.GetChunk(x, z - 1);
                break;
            case BlockAdjacency.EAST:
                neighbor = _worldChunks.GetChunk(x + 1, z);
                break;
            case BlockAdjacency.WEST:
                neighbor = _worldChunks.GetChunk(x - 1, z);
                break;
        }
        if (neighbor == null)
            return;

        for (int i = 0; i < _chunkSize; i++)
        {
            Block currentBlock = null;
            Block neighborBlock = null;
            switch (direction)
            {
                case BlockAdjacency.NORTH:
                    currentBlock = current.GetSurfaceBlock(i, _chunkSize - 1);
                    neighborBlock = neighbor.GetSurfaceBlock(i, 0);
                    break;
                case BlockAdjacency.SOUTH:
                    currentBlock = current.GetSurfaceBlock(i, 0);
                    neighborBlock = neighbor.GetSurfaceBlock(i, _chunkSize - 1);
                    break;
                case BlockAdjacency.EAST:
                    currentBlock = current.GetSurfaceBlock(_chunkSize - 1, i);
                    neighborBlock = neighbor.GetSurfaceBlock(0, i);
                    break;
                case BlockAdjacency.WEST:
                    currentBlock = current.GetSurfaceBlock(0, i);
                    neighborBlock = neighbor.GetSurfaceBlock(_chunkSize - 1, i);
                    break;
            }

            int blocksToFill = currentBlock.y - neighborBlock.y - 1;
            if (blocksToFill > 0)
            {
                for (int y = currentBlock.y - 1; y >= currentBlock.y - blocksToFill; y--)
                {
                    Block fill = BlockFactory.CreateBlock(currentBlock.x, y, currentBlock.z, BlockType.ROCK, new Vector3(currentBlock.x, y, currentBlock.z));
                    current.SetBlock(fill);
                }
            }
        }
    }

    private void CreateChunkMeshes()
    {
        foreach(var chunk in _worldChunks.chunks)
        {
            chunk.CreateMeshData(_worldChunks);
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
