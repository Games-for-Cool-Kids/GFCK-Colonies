using UnityEngine;
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

    void Start()
    {
        Generate();
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Generate();
        }
    }

    private void Generate()
    {
        WorldGenerator generator = new();
        generator.GenerateWorld(GetChunkStats(), LoadData);
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
