using System.Collections.Generic;
using UnityEngine;

public static class WorldMeshUtilities
{
    public static void CreateFaceUp(WorldMeshData data, Vector3 origin)
    {
        int oldVertCount = data.vertices.Count;

        Vector3[] verts = new Vector3[4];
        verts[0] = origin + new Vector3(-0.5f, 0.5f, 0.5f);
        verts[1] = origin + new Vector3(0.5f, 0.5f, 0.5f);
        verts[2] = origin + new Vector3(0.5f, 0.5f, -0.5f);
        verts[3] = origin + new Vector3(-0.5f, 0.5f, -0.5f);

        data.vertices.AddRange(verts);

        int[] tris = new int[6];
        tris[0] = oldVertCount;
        tris[1] = oldVertCount + 1;
        tris[2] = oldVertCount + 2;

        tris[3] = oldVertCount + 2;
        tris[4] = oldVertCount + 3;
        tris[5] = oldVertCount;

        data.triangles.AddRange(tris);
    }
}

