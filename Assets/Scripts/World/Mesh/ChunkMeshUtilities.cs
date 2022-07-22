using System.Collections.Generic;
using UnityEngine;

public static class ChunkMeshUtilities
{
    private static float _faceTexScale = 0.25f; // Texture is subdivided in 4x4 faces

    public static void CreateFace(Vector3[] faceVertices, ChunkMeshData data, BlockFace face)
    {
        data.vertices.AddRange(faceVertices);

        AddFaceTriangles(data, faceVertices);
        AddUVs(data, face);
    }

    public static void CreateFaceUp(ChunkMeshData data, Vector3 origin, BlockType type)
    {
        Vector3[] verts = new Vector3[4];
        verts[0] = origin + new Vector3(-0.5f, 0.5f, 0.5f);
        verts[1] = origin + new Vector3(0.5f, 0.5f, 0.5f);
        verts[2] = origin + new Vector3(0.5f, 0.5f, -0.5f);
        verts[3] = origin + new Vector3(-0.5f, 0.5f, -0.5f);

        CreateFace(verts, data, CreateBlockFace(type, BlockFace.DIRECTION.TOP));
    }

    public static void CreateFaceDown(ChunkMeshData data, Vector3 origin, BlockType type)
    {
        Vector3[] verts = new Vector3[4];
        verts[0] = origin + new Vector3(-0.5f, -0.5f, -0.5f);
        verts[1] = origin + new Vector3(0.5f, -0.5f, -0.5f);
        verts[2] = origin + new Vector3(0.5f, -0.5f, 0.5f);
        verts[3] = origin + new Vector3(-0.5f, -0.5f, 0.5f);

        CreateFace(verts, data, CreateBlockFace(type));
    }

    public static void CreateFaceLeft(ChunkMeshData data, Vector3 origin, BlockType type)
    {
        Vector3[] verts = new Vector3[4];
        verts[0] = origin + new Vector3(-0.5f, -0.5f, -0.5f);
        verts[1] = origin + new Vector3(-0.5f, -0.5f, 0.5f);
        verts[2] = origin + new Vector3(-0.5f, 0.5f, 0.5f);
        verts[3] = origin + new Vector3(-0.5f, 0.5f, -0.5f);

        CreateFace(verts, data, CreateBlockFace(type));
    }

    public static void CreateFaceRight(ChunkMeshData data, Vector3 origin, BlockType type)
    {
        Vector3[] verts = new Vector3[4];
        verts[0] = origin + new Vector3(0.5f, 0.5f, -0.5f);
        verts[1] = origin + new Vector3(0.5f, 0.5f, 0.5f);
        verts[2] = origin + new Vector3(0.5f, -0.5f, 0.5f);
        verts[3] = origin + new Vector3(0.5f, -0.5f, -0.5f);

        CreateFace(verts, data, CreateBlockFace(type));
    }

    public static void CreateFaceForward(ChunkMeshData data, Vector3 origin, BlockType type)
    {
        Vector3[] verts = new Vector3[4];
        verts[0] = origin + new Vector3(-0.5f, -0.5f, 0.5f);
        verts[1] = origin + new Vector3(0.5f, -0.5f, 0.5f);
        verts[2] = origin + new Vector3(0.5f, 0.5f, 0.5f);
        verts[3] = origin + new Vector3(-0.5f, 0.5f, 0.5f);

        CreateFace(verts, data, CreateBlockFace(type));
    }

    public static void CreateFaceBackward(ChunkMeshData data, Vector3 origin, BlockType type)
    {
        Vector3[] verts = new Vector3[4];
        verts[0] = origin + new Vector3(-0.5f, 0.5f, -0.5f);
        verts[1] = origin + new Vector3(0.5f, 0.5f, -0.5f);
        verts[2] = origin + new Vector3(0.5f, -0.5f, -0.5f);
        verts[3] = origin + new Vector3(-0.5f, -0.5f, -0.5f);

        CreateFace(verts, data, CreateBlockFace(type));
    }

    public static void AddFaceTriangles(ChunkMeshData data, Vector3[] vertices)
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

    public static void AddUVs(ChunkMeshData data, BlockFace face)
    {
        Vector2[] uvs = new Vector2[4];

        Rect uvRect = new Rect();
        uvRect.x = face.x * _faceTexScale;
        uvRect.y = face.y * _faceTexScale;
        uvRect.width = _faceTexScale;
        uvRect.height = _faceTexScale;

        // Fix issue with block face UV edge bleeding. TODO: find better fix
        Vector2 pixelErrorOffset = new Vector2(0.01f, 0.01f);
        uvRect.min += pixelErrorOffset;
        uvRect.max -= pixelErrorOffset;

        uvs[0] = uvRect.min;
        uvs[1] = new Vector2(uvRect.xMax, uvRect.yMin);
        uvs[2] = uvRect.max;
        uvs[3] = new Vector2(uvRect.xMin, uvRect.yMax);

        data.uv.AddRange(uvs);
    }

    public static BlockFace CreateBlockFace(BlockType type, BlockFace.DIRECTION direction = BlockFace.DIRECTION.SIDE)
    {
        switch (type)
        {
            case BlockType.WATER:
                return new BlockFace() { x = 1, y = 1, direction = direction };
            case BlockType.SAND:
                return new BlockFace() { x = 1, y = 2, direction = direction };
            case BlockType.SNOW:
                return new BlockFace() { x = 0, y = 3, direction = direction };
            case BlockType.ROCK:
                return new BlockFace() { x = 3, y = 3, direction = direction };
            case BlockType.GROUND:
            case BlockType.GRASS:
            default:
                switch (direction)
                {
                    case BlockFace.DIRECTION.TOP:
                        return new BlockFace() { x = 0, y = 2, direction = direction };
                    case BlockFace.DIRECTION.SIDE:
                    default:
                        return new BlockFace() { x = 2, y = 2, direction = direction };
                }
        }
    }
}

