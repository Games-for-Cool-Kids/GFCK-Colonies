using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    public int chunkSize = 32;
    public int worldChunkWidth { get; private set; } // Nr of chunkGrid in width and length, as world is a square.

    public Material material;

    public WorldVariable worldVariable;
    public ChunkGrid chunkGrid;
    public GameObject[,] chunkObjects;

    private WorldGenerator worldGenerator = null;

    private void Start()
    {
        StartCreateWorld();
    }

    void Update()
    {
        if(worldGenerator != null)
            RunWorldGeneration();

        if(Input.GetMouseButtonDown(0))
        {
            Block block = GetBlockUnderMouse();
            if (block != null)
                DigBlock(block);
        }
    }

    public void StartCreateWorld()
    {
        ResetWorld();

        worldChunkWidth = Mathf.FloorToInt(worldVariable.size / chunkSize);

        worldGenerator = new(chunkSize, worldChunkWidth, worldVariable, TakeGeneratedWorld); // Creates world using multithreading. We need to wait for it to finish to use the world.
    }

    public void RunWorldGeneration()
    {
        if (worldGenerator.worldGenCompleted)
            worldGenerator.NotifyCompleted();
        else
            worldGenerator.Run();
    }

    private void TakeGeneratedWorld(ChunkGrid generatedChunkGrid)
    {
        chunkGrid = generatedChunkGrid;
        chunkObjects = new GameObject[worldChunkWidth, worldChunkWidth];

        foreach (var chunk in generatedChunkGrid.chunks)
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
        chunkObjects[chunk.x, chunk.z] = newChunkObject;

        MeshFilter meshFilter = newChunkObject.AddComponent<MeshFilter>();
        meshFilter.mesh = chunk.TakeMesh();

        MeshRenderer renderer = newChunkObject.AddComponent<MeshRenderer>();
        renderer.material = material;

        newChunkObject.AddComponent<MeshCollider>();
    }

    private void ResetWorld()
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
        return chunkGrid.GetChunkAt(worldPos);
    }

    public Chunk GetChunk(int x, int z)
    {
        if(x < 0 || x >= worldChunkWidth
        || z < 0 || z >= worldChunkWidth)
            return null;

        return chunkGrid.chunks[x, z];
    }

    public Block GetBlockUnderMouse(bool ignoreOtherLayers = false)
    {
        if (UIUtil.IsMouseOverUI()) // Always ignore UI
            return null;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask layerMask = ignoreOtherLayers ? LayerMask.GetMask(GlobalDefines.worldLayerName) : ~0;

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
            return GetBlockFromRayHit(hit);

        return null;
    }

    public void DigBlock(Block block)
    {
        chunkGrid.DestroyBlock(block);

        UpdateChangedChunkMeshes();
    }

    private void UpdateChangedChunkMeshes()
    {
        foreach (var chunk in chunkGrid.chunks)
        {
            if (chunk.meshChanged)
            {
                GameObject chunkObject = chunkObjects[chunk.x, chunk.z];
                chunkObject.GetComponent<MeshFilter>().mesh = chunk.TakeMesh();
                chunkObject.GetComponent<MeshCollider>().sharedMesh = chunkObject.GetComponent<MeshFilter>().mesh;
            }
        }
    }
}
