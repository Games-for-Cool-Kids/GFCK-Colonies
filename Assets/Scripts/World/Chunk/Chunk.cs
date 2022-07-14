using UnityEngine;

public class Chunk
{
    public Vector3 position;
    public BlockGrid grid;

    public ChunkMeshData meshData;

    public Chunk(Vector3 position, int size, int maxY)
    {
        this.position = position;
        this.grid = new BlockGrid(size, maxY, size);
        this.meshData = new ChunkMeshData();
    }
}
