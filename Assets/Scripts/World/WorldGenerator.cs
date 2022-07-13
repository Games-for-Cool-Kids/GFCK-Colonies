using UnityEngine;
using SimplexNoise;

public class WorldGenerator : MonoBehaviour
{
    private MeshFilter _meshFilter;

    public int maxX = 16;
    public int maxZ = 16;

    public float BaseNoise = 0.02f;
    public float BaseHeight = -5f;
    public float BaseNoiseHeight = 4;
    public int Elevation = 15;
    public float Frequency = 0.005f;

    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();

        WorldMeshData meshData = new WorldMeshData();

        LoadMeshData(CreateWorld());
    }

    int GetNoise(int x, int y, int z, float scale, int max)
    {
        return Mathf.FloorToInt((Noise.Generate(x * scale, y * scale, z * scale) + 1) * (max / 2.0f));
    }

    WorldMeshData CreateWorld()
    {
        WorldMeshData worldData = new WorldMeshData();
        for(int x = 0; x < maxX; x++)
        {
            for (int z = 0; z < maxZ; z++)
            {
                Vector3 blockPos = Vector3.zero;
                blockPos.x = x;
                blockPos.z = z;

                float height = BaseHeight;
                height += GetNoise(x, 0, z, Frequency, Elevation);
                blockPos.y = height;

                WorldMeshUtilities.CreateFaceUp(worldData, blockPos);
            }
        }

        return worldData;
    }

    public void LoadMeshData(WorldMeshData data)
    {
        Mesh mesh = new Mesh()
        {
            vertices = data.vertices.ToArray(),
            uv = data.uv.ToArray(),
            triangles = data.triangles.ToArray()
        };
        _meshFilter.mesh = mesh;
    }
}
