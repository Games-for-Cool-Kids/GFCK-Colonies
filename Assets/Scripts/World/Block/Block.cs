using UnityEngine;

public class Block
{
    public int x, y, z;

    public bool filled = false;

    public Vector3 GetPosition()
    {
        return new Vector3(x, y, z);
    }

    public virtual void CreateMesh(WorldMeshData worldData, BlockGrid grid)
    {
        Vector3 blockPos = GetPosition();

        WorldMeshUtilities.CreateFaceUp(worldData, blockPos); // Always create up.

        Block northBlock = grid.GetAdjacentBlock(this, BlockGrid.Adjacency.NORTH);
        if (northBlock == null || !northBlock.filled)
            WorldMeshUtilities.CreateFaceForward(worldData, blockPos);

        Block southBlock = grid.GetAdjacentBlock(this, BlockGrid.Adjacency.SOUTH);
        if (southBlock == null || !southBlock.filled)
            WorldMeshUtilities.CreateFaceBackward(worldData, blockPos);

        Block westBlock = grid.GetAdjacentBlock(this, BlockGrid.Adjacency.WEST);
        if (westBlock == null || !westBlock.filled)
            WorldMeshUtilities.CreateFaceLeft(worldData, blockPos);

        Block eastBlock = grid.GetAdjacentBlock(this, BlockGrid.Adjacency.EAST);
        if (eastBlock == null || !eastBlock.filled)
            WorldMeshUtilities.CreateFaceRight(worldData, blockPos);

        // We don't create a bottom face.
    }
}
