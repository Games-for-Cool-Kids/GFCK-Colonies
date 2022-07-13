using UnityEngine;
using SimplexNoise;

public class WorldGenerator : MonoBehaviour
{
    private MeshFilter _meshFilter;

    public int MaxX = 16;
    public int MaxY = 16;
    public int MaxZ = 16;

    public float BaseNoise = 0.02f;
    public float BaseNoiseHeight = 4;
    public float Frequency = 0.005f;

    BlockGrid Grid;

    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();

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
        GameObject.Destroy(_meshFilter.mesh); // Delete old mesh

        WorldMeshData worldData = CreateWorld();
        LoadFilledBlocks(worldData);
        LoadMeshData(worldData);
    }

    private int GetNoise(int x, int y, int z, float scale, int max)
    {
        return Mathf.FloorToInt((Noise.Generate(x * scale, y * scale, z * scale) + 1) * (max / 2.0f));
    }

    private WorldMeshData CreateWorld()
    {
        Grid = new BlockGrid(MaxX, MaxY, MaxZ);

        WorldMeshData worldData = new WorldMeshData();
        for(int x = 0; x < MaxX; x++)
        {
            for (int z = 0; z < MaxZ; z++)
            {
                float height = GetNoise(x, 0, z, Frequency, MaxY);

                Block newBlock = new Block()
                {
                    x = x,
                    y = Mathf.RoundToInt(height),
                    z = z,
                    filled = true,
                };
                Grid.SetBlock(newBlock.x, newBlock.y, newBlock.z, newBlock);
            }
        }

        return worldData;
    }

    private void LoadFilledBlocks(WorldMeshData data)
    {
        foreach (Block filledBlock in Grid.GetFilledBlocks())
        {
            filledBlock.Load(data, Grid);
        }
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

        _meshFilter.mesh = mesh;
    }
}
