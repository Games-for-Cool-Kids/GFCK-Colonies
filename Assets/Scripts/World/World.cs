using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    public int chunkSize = 32;
    public int worldChunkWidth { get; private set; } // Nr of chunks in width and length, as world is a square.

    public Material material;

    public WorldVariable worldVariable;
    public Chunk[,] chunks;
    public BlockGrid worldGrid;

    private WorldGenerator worldGenerator = null;

    private void Start()
    {
        StartCreateWorld();
    }

    void Update()
    {
        if(worldGenerator != null)
            RunWorldGeneration();
    }

    public void StartCreateWorld()
    {
        Reset();

        worldChunkWidth = Mathf.FloorToInt(worldVariable.size / chunkSize);

        worldGenerator = new(chunkSize, worldChunkWidth, worldVariable, TakeGeneratedWorld); // Creates world using multithreading. We need to wait for it to finish to use the world.
        //worldGenerator.fin
    }

    public void RunWorldGeneration()
    {
        if (worldGenerator.worldGenCompleted)
            worldGenerator.NotifyCompleted();
        else
            worldGenerator.Run();
    }

    private void TakeGeneratedWorld(Chunk[,] chunks, BlockGrid worldGrid)
    {
        this.worldGrid = worldGrid;
        this.chunks = chunks;

        foreach (var chunk in chunks)
        {
            CreateChunkObject(chunk);
        }

        worldGenerator = null; // Stops generator from running.
    }

    private void CreateChunkObject(Chunk chunk)
    {
        GameObject newChunkObject = new GameObject("Chunk" + chunk.origin.ToString());
        newChunkObject.isStatic = true;
        newChunkObject.layer = LayerMask.NameToLayer(GlobalDefines.worldLayerName);
        newChunkObject.transform.parent = transform;
        newChunkObject.transform.position = chunk.origin;

        MeshFilter meshFilter = newChunkObject.AddComponent<MeshFilter>();
        meshFilter.mesh = LoadChunkMesh(chunk);

        MeshRenderer renderer = newChunkObject.AddComponent<MeshRenderer>();
        renderer.material = material;

        newChunkObject.AddComponent<MeshCollider>();
    }

    private Mesh LoadChunkMesh(Chunk chunk)
    {
        Mesh chunkMesh = new Mesh()
        {
            vertices = chunk.meshData.vertices.ToArray(),
            uv = chunk.meshData.uv.ToArray(),
            triangles = chunk.meshData.triangles.ToArray()
        };

        chunkMesh.RecalculateNormals();

        return chunkMesh;
    }

    private void Reset()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public Block GetBlockFromRayHit(RaycastHit hit)
    {
        // Rays intersect with surface. Because the surface is touching, but not inside the box, we need to use the normal to check the position inside the block.
        return GetBlockAt(hit.point - hit.normal / 2);
    }

    // Expects a position inside of the block.
    public Block GetBlockAt(Vector3 worldPos)
    {
        var chunk = GetChunkAt(worldPos);
        if (chunk == null)
        {
            return null;
        }

        return chunk.GetBlockAt(worldPos);
    }

    public Chunk GetChunkAt(Vector3 worldPos)
    {
        Vector3 relativePos = worldPos - transform.position;
        relativePos += new Vector3(0.5f, 0.5f, 0.5f); // We need offset of half a block. Origin is middle of first block.

        int chunkX = Mathf.FloorToInt(relativePos.x / this.chunkSize);
        int chunkZ = Mathf.FloorToInt(relativePos.z / this.chunkSize);

        return GetChunk(chunkX, chunkZ);
    }

    public Chunk GetChunk(int x, int z)
    {
        if(x < 0 || x >= worldChunkWidth
        || z < 0 || z >= worldChunkWidth)
            return null;

        return chunks[x, z];
    }

    public Block GetBlockUnderMouse(bool ignoreOtherLayers = false)
    {
        if (UIUtil.IsMouseOverUI()) // Always ignore UI
            return null;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = ignoreOtherLayers ? ~LayerMask.NameToLayer(GlobalDefines.worldLayerName) : ~0;

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
            return GetBlockFromRayHit(hit);

        return null;
    }
}
