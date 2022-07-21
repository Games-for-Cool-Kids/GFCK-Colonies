using UnityEngine;

public class Chunk
{
    public int x;
    public int z;
    public Vector3 origin;
    public BlockGrid grid;

    public ChunkMeshData meshData;
    public bool meshChanged { get; private set; }

    public Chunk(int x, int z, Vector3 position, int size, int maxY)
    {
        this.x = x;
        this.z = z;
        this.origin = position;
        this.grid = new BlockGrid(size, maxY, size);
        this.meshData = new ChunkMeshData();
        this.meshChanged = false;
    }

    public Block GetBlockAt(Vector3 worldPos)
    {
        worldPos -= origin; // Make relative to chunk.

        int blockX = Mathf.RoundToInt(worldPos.x);
        int blockY = Mathf.RoundToInt(worldPos.y);
        int blockZ = Mathf.RoundToInt(worldPos.z);

        return grid.GetBlock(blockX, blockY, blockZ);
    }

    public void DestroyBlock(Block block)
    {
        grid.DestroyBlock(block);
    }

    public void CreateMeshData()
    {
        meshData = new ChunkMeshData();
        meshChanged = true;

        foreach (Block filledBlock in grid.GetFilledBlocks())
            filledBlock.CreateMesh(meshData, grid);
    }

    public Mesh TakeMesh()
    {
        meshChanged = false;

        Mesh chunkMesh = new Mesh()
        {
            vertices = meshData.vertices.ToArray(),
            uv = meshData.uv.ToArray(),
            triangles = meshData.triangles.ToArray()
        };
        chunkMesh.RecalculateNormals();

        return chunkMesh;
    }

    public void CreateBlocksUnder(int x, int y, int z, int amount)
    {
        grid.CreateBlocksUnder(grid.GetBlock(x, y, z), amount);
    }
}
