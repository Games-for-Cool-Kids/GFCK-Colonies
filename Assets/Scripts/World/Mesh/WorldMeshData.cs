using System.Collections.Generic;
using UnityEngine;

public class WorldMeshData
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uv = new List<Vector2>();
    public Vector3 origin;
}
