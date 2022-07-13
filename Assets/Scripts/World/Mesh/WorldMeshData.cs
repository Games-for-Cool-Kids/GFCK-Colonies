using System.Collections.Generic;
using UnityEngine;

public class WorldMeshData
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uv = new List<Vector2>();

    public void AddVertices(List<Vector3> newVertices)
    {
        vertices.AddRange(newVertices);
    }

    public void AddTriangles(List<int> newTriangles)
    {
        triangles.AddRange(newTriangles);
    }

    public void AddUVs(List<Vector2> newUV)
    {
        uv.AddRange(newUV);
    }
}
