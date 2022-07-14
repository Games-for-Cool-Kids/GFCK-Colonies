using UnityEngine;

public class Chunk
{
    public int x;
    public int z;
    public Vector3 origin;
    public BlockGrid grid;

    public ChunkMeshData meshData;

    public Chunk(int x, int z, Vector3 position, int size, int maxY)
    {
        this.x = x;
        this.z = z;
        this.origin = position;
        this.grid = new BlockGrid(size, maxY, size);
        this.meshData = new ChunkMeshData();
    }

    public Block GetBlockAt(Vector3 worldPos)
    {
        worldPos -= origin; // Make relative to chunk.

        int blockX = Mathf.RoundToInt(worldPos.x);
        int blockY = Mathf.RoundToInt(worldPos.y);
        int blockZ = Mathf.RoundToInt(worldPos.z);

        return grid.GetBlock(blockX, blockY, blockZ);
    }
}
