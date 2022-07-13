using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class World : MonoBehaviour
{
    public int MaxX = 16;
    public int MaxY = 16;
    public int MaxZ = 16;

    public float BaseNoise = 0.02f;
    public float BaseNoiseHeight = 4;
    public float Frequency = 0.005f;

    public BlockGrid BlockGrid;

    public int MaxWorkers = 4;
    List<WorldGenerator> ToDoWorkers = new List<WorldGenerator>();
    List<WorldGenerator> CurrentWorkers = new List<WorldGenerator>();

    private void Start()
    {
        RequestWorldGeneration();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RequestWorldGeneration();
        }

        int i = 0;
        while(i < CurrentWorkers.Count)
        {
            if(CurrentWorkers[i].GenerationCompleted)
            {
                CurrentWorkers[i].NotifyCompleted();
                CurrentWorkers.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        if(ToDoWorkers.Count > 0
        && CurrentWorkers.Count < MaxWorkers)
        {
            WorldGenerator generator = ToDoWorkers[0];
            ToDoWorkers.RemoveAt(0);
            CurrentWorkers.Add(generator);

            Thread workerThread = new Thread(generator.GenerateWorld);
            workerThread.Start();
        }
    }

    public void RequestWorldGeneration()
    {
        WorldGenerator generator = new(GetChunkStats(), LoadData);
        ToDoWorkers.Add(generator);
    }

    private void LoadData(BlockGrid grid, WorldMeshData data)
    {
        BlockGrid = grid;
        LoadMeshData(data);
    }

    public void LoadMeshData(WorldMeshData data)
    {
        Mesh mesh = new Mesh()
        {
            vertices = data.vertices.ToArray(),
            uv = data.uv.ToArray(),
            triangles = data.triangles.ToArray()
        };

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private WorldChunkStats GetChunkStats()
    {
        return new WorldChunkStats
        {
            MaxX = MaxX,
            MaxY = MaxY,
            MaxZ = MaxZ,
            BaseNoise = BaseNoise,
            BaseNoiseHeight = BaseNoiseHeight,
            Frequency = Frequency,
        };
    }
}
