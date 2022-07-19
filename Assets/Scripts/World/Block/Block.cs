using UnityEngine;

public class Block
{
    public enum Type
    {
        GROUND,
        GRASS,
        WATER,
        SAND,
        SNOW,
    }

    public int x, y, z; // Position in chunk. !Not world!

    public bool filled = false;

    public Vector3 worldPosition;

    public Type type;

    public Block(int x, int y, int z, bool filled, Type type, Vector3 worldPosition)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.filled = filled;
        this.worldPosition = worldPosition;
        this.type = type;
    }

    public virtual void CreateMesh(ChunkMeshData worldData, BlockGrid grid)
    {
        Vector3 blockPos = new Vector3(x, y, z);

        ChunkMeshUtilities.CreateFaceUp(worldData, blockPos, type); // Always create up.

        Block northBlock = grid.GetAdjacentBlock(this, BlockGrid.Adjacency.NORTH);
        if (northBlock == null || !northBlock.filled)
            ChunkMeshUtilities.CreateFaceForward(worldData, blockPos, type);

        Block southBlock = grid.GetAdjacentBlock(this, BlockGrid.Adjacency.SOUTH);
        if (southBlock == null || !southBlock.filled)
            ChunkMeshUtilities.CreateFaceBackward(worldData, blockPos, type);

        Block westBlock = grid.GetAdjacentBlock(this, BlockGrid.Adjacency.WEST);
        if (westBlock == null || !westBlock.filled)
            ChunkMeshUtilities.CreateFaceLeft(worldData, blockPos, type);

        Block eastBlock = grid.GetAdjacentBlock(this, BlockGrid.Adjacency.EAST);
        if (eastBlock == null || !eastBlock.filled)
            ChunkMeshUtilities.CreateFaceRight(worldData, blockPos, type);

        // We don't create a bottom face.
    }

    // Determines if it can be used for pathfinding.
    public bool IsWalkable()
    {
        return filled;
    }

    public Vector3 GetWorldPositionOnTop()
    {
        return worldPosition + Vector3.up / 2;
    }
}
