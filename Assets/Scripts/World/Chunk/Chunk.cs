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

    public BlockData GetBlockAt(Vector3 worldPos)
    {
        worldPos -= origin; // Make relative to chunk.

        int blockX = Mathf.RoundToInt(worldPos.x);
        int blockY = Mathf.RoundToInt(worldPos.y);
        int blockZ = Mathf.RoundToInt(worldPos.z);

        return grid.GetBlock(blockX, blockY, blockZ);
    }

    public void DestroyBlock(BlockData block)
    {
        grid.DestroyBlock(block);
    }

    public void CreateMeshData()
    {
        meshData = new ChunkMeshData();
        meshChanged = true;

        foreach (BlockData filledBlock in grid.GetFilledBlocks())
        {
            AddBlockToChunkMesh(filledBlock, this);
        }
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

    public static void AddBlockToChunkMesh(BlockData block, Chunk chunk)
    {
        AddBlockFaceToMeshIfVisible(block, chunk, BlockGrid.Adjacency.ABOVE);
        AddBlockFaceToMeshIfVisible(block, chunk, BlockGrid.Adjacency.NORTH);
        AddBlockFaceToMeshIfVisible(block, chunk, BlockGrid.Adjacency.SOUTH);
        AddBlockFaceToMeshIfVisible(block, chunk, BlockGrid.Adjacency.EAST);
        AddBlockFaceToMeshIfVisible(block, chunk, BlockGrid.Adjacency.WEST);
    }

    private static void AddBlockFaceToMeshIfVisible(BlockData block, Chunk chunk, BlockGrid.Adjacency adjacency)
    {
        BlockData neighbor = chunk.grid.GetAdjacentBlock(block, adjacency);
        if (neighbor == null
        || !neighbor.filled)
        {
            Vector3 localPos = BlockCode.GetLocalPosition(block);

            switch (adjacency)
            {
                case BlockGrid.Adjacency.NORTH:
                    ChunkMeshUtilities.CreateFaceForward(chunk.meshData, localPos, block.type);
                    break;
                case BlockGrid.Adjacency.SOUTH:
                    ChunkMeshUtilities.CreateFaceBackward(chunk.meshData, localPos, block.type);
                    break;
                case BlockGrid.Adjacency.EAST:
                    ChunkMeshUtilities.CreateFaceRight(chunk.meshData, localPos, block.type);
                    break;
                case BlockGrid.Adjacency.WEST:
                    ChunkMeshUtilities.CreateFaceLeft(chunk.meshData, localPos, block.type);
                    break;
                case BlockGrid.Adjacency.ABOVE:
                    ChunkMeshUtilities.CreateFaceUp(chunk.meshData, localPos, block.type);
                    break;
                case BlockGrid.Adjacency.BELOW:  // Maybe we can even skip the backside of blocks too.
                default:                         // We don't create a bottom face. Since the camera can never see the bottom of blocks.
                    break;
            }
        }
    }
}
