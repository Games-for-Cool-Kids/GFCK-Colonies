using UnityEngine;

public class Block
{
    public int x, y, z; // Position in chunk. !Not world!

    public bool filled = false;

    public Vector3 worldPosition;

    public Block(int x, int y, int z, bool filled, Vector3 worldPosition)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.filled = filled;
        this.worldPosition = worldPosition;
    }

    public virtual void CreateMesh(ChunkMeshData worldData, BlockGrid grid)
    {
        Vector3 blockPos = new Vector3(x, y, z);

        ChunkMeshUtilities.CreateFaceUp(worldData, blockPos); // Always create up.

        Block northBlock = grid.GetAdjacentBlock(this, BlockGrid.Adjacency.NORTH);
        if (northBlock == null || !northBlock.filled)
            ChunkMeshUtilities.CreateFaceForward(worldData, blockPos);

        Block southBlock = grid.GetAdjacentBlock(this, BlockGrid.Adjacency.SOUTH);
        if (southBlock == null || !southBlock.filled)
            ChunkMeshUtilities.CreateFaceBackward(worldData, blockPos);

        Block westBlock = grid.GetAdjacentBlock(this, BlockGrid.Adjacency.WEST);
        if (westBlock == null || !westBlock.filled)
            ChunkMeshUtilities.CreateFaceLeft(worldData, blockPos);

        Block eastBlock = grid.GetAdjacentBlock(this, BlockGrid.Adjacency.EAST);
        if (eastBlock == null || !eastBlock.filled)
            ChunkMeshUtilities.CreateFaceRight(worldData, blockPos);

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
