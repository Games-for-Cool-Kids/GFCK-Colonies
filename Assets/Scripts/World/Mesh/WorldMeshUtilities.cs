using System.Collections.Generic;
using UnityEngine;

public static class WorldMeshUtilities
{
    public static void CreateFace(Vector3[] faceVertices, WorldMeshData data)
    {
        data.vertices.AddRange(faceVertices);

        AddFaceTriangles(data, faceVertices);
        AddUVs(data);
    }

    public static void CreateFaceUp(WorldMeshData data, Vector3 origin)
    {
        Vector3[] verts = new Vector3[4];
        verts[0] = origin + new Vector3(-0.5f, 0.5f, 0.5f);
        verts[1] = origin + new Vector3(0.5f, 0.5f, 0.5f);
        verts[2] = origin + new Vector3(0.5f, 0.5f, -0.5f);
        verts[3] = origin + new Vector3(-0.5f, 0.5f, -0.5f);

        CreateFace(verts, data);
    }

    public static void CreateFaceDown(WorldMeshData data, Vector3 origin)
    {
        Vector3[] verts = new Vector3[4];
        verts[0] = origin + new Vector3(-0.5f, -0.5f, -0.5f);
        verts[1] = origin + new Vector3(0.5f, -0.5f, -0.5f);
        verts[2] = origin + new Vector3(0.5f, -0.5f, 0.5f);
        verts[3] = origin + new Vector3(-0.5f, -0.5f, 0.5f);

        CreateFace(verts, data);
    }

    public static void CreateFaceLeft(WorldMeshData data, Vector3 origin)
    {
        Vector3[] verts = new Vector3[4];
        verts[0] = origin + new Vector3(-0.5f, -0.5f, -0.5f);
        verts[1] = origin + new Vector3(-0.5f, -0.5f, 0.5f);
        verts[2] = origin + new Vector3(-0.5f, 0.5f, 0.5f);
        verts[3] = origin + new Vector3(-0.5f, 0.5f, -0.5f);

        CreateFace(verts, data);
    }

    public static void CreateFaceRight(WorldMeshData data, Vector3 origin)
    {
        Vector3[] verts = new Vector3[4];
        verts[0] = origin + new Vector3(0.5f, 0.5f, -0.5f);
        verts[1] = origin + new Vector3(0.5f, 0.5f, 0.5f);
        verts[2] = origin + new Vector3(0.5f, -0.5f, 0.5f);
        verts[3] = origin + new Vector3(0.5f, -0.5f, -0.5f);

        CreateFace(verts, data);
    }

    public static void CreateFaceForward(WorldMeshData data, Vector3 origin)
    {
        Vector3[] verts = new Vector3[4];
        verts[0] = origin + new Vector3(-0.5f, -0.5f, 0.5f);
        verts[1] = origin + new Vector3(0.5f, -0.5f, 0.5f);
        verts[2] = origin + new Vector3(0.5f, 0.5f, 0.5f);
        verts[3] = origin + new Vector3(-0.5f, 0.5f, 0.5f);

        CreateFace(verts, data);
    }

    public static void CreateFaceBackward(WorldMeshData data, Vector3 origin)
    {
        Vector3[] verts = new Vector3[4];
        verts[0] = origin + new Vector3(-0.5f, 0.5f, -0.5f);
        verts[1] = origin + new Vector3(0.5f, 0.5f, -0.5f);
        verts[2] = origin + new Vector3(0.5f, -0.5f, -0.5f);
        verts[3] = origin + new Vector3(-0.5f, -0.5f, -0.5f);

        CreateFace(verts, data);
    }

    public static void AddFaceTriangles(WorldMeshData data, Vector3[] vertices)
    {
        int offset = data.vertices.Count - vertices.Length;

        int[] tris = new int[6];
        tris[0] = offset;
        tris[1] = offset + 1;
        tris[2] = offset + 2;

        tris[3] = offset + 2;
        tris[4] = offset + 3;
        tris[5] = offset;

        data.triangles.AddRange(tris);
    }

    public static void AddUVs(WorldMeshData data)
    {
        Vector2[] uvs = new Vector2[4];

        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(0, 1);
        uvs[2] = new Vector2(1, 1);
        uvs[3] = new Vector2(1, 0);

        data.uv.AddRange(uvs);
    }
}

