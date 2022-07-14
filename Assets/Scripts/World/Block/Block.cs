using UnityEngine;

public class Block
{
    public int x, y, z;

    public bool filled = false;

    public Vector3 GetPosition()
    {
        return new Vector3(x, y, z);
    }

    public virtual void CreateMesh(ChunkMeshData worldData, BlockGrid grid)
    {
        Vector3 blockPos = GetPosition();

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
}
