using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    private MeshFilter _meshFilter;

    public int maxX = 16;
    public int maxZ = 16;

    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();

        WorldMeshData meshData = new WorldMeshData();

        LoadMeshData(CreateWorld());
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
                blockPos.y = Random.Range(0, 3);
                blockPos.z = z;

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
