using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class World : MonoBehaviour
{
    public int maxX = 16;
    public int maxY = 16;
    public int maxZ = 16;

    public float baseNoise = 0.02f;
    public float baseNoiseHeight = 4;
    public float frequency = 0.005f;

    public BlockGrid blockGrid;

    public int maxWorkers = 4;
    List<WorldGenerator> toDoWorkers = new List<WorldGenerator>();
    List<WorldGenerator> currentWorkers = new List<WorldGenerator>();

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
        while(i < currentWorkers.Count)
        {
            if(currentWorkers[i].GenerationCompleted)
            {
                currentWorkers[i].NotifyCompleted();
                currentWorkers.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        if(toDoWorkers.Count > 0
        && currentWorkers.Count < maxWorkers)
        {
            WorldGenerator generator = toDoWorkers[0];
            toDoWorkers.RemoveAt(0);
            currentWorkers.Add(generator);

            Thread workerThread = new Thread(generator.GenerateWorld);
            workerThread.Start();
        }
    }

    public void RequestWorldGeneration()
    {
        WorldGenerator generator = new(GetChunkStats(), LoadData);
        toDoWorkers.Add(generator);
    }

    private void LoadData(BlockGrid grid, WorldMeshData data)
    {
        blockGrid = grid;
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
            maxX = maxX,
            maxY = maxY,
            maxZ = maxZ,
            baseNoise = baseNoise,
            baseNoiseHeight = baseNoiseHeight,
            frequency = frequency,
        };
    }
}
