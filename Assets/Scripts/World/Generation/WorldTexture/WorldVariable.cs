using UnityEngine;

[CreateAssetMenu(menuName = "GameWorld/GameWorld Variable")]
public class WorldVariable : ScriptableObject
{
    public int size { get; private set; } // square size x size.
    public int height { get; private set; }

    public Texture2D texture { get; private set; }
    public Sprite worldSprite { get; private set; }

    public WorldGenBlockNode[,] blockGrid;
    public WorldGenResourceNode[,] resourceGrid;

    public void Init(int size, int height)
    {
        this.size = size;
        this.height = height;

        texture = new Texture2D(size, size);

        Rect rect = new Rect(0, 0, size, size);
        worldSprite = Sprite.Create(texture, rect, Vector2.zero, 100, 0, SpriteMeshType.FullRect);

        blockGrid = new WorldGenBlockNode[size, size];
        resourceGrid = new WorldGenResourceNode[size, size];
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                blockGrid[x, y] = new(x, y);
                resourceGrid[x, y] = new(x, y);
            }
    }

    public void ApplyTexture()
    {
        texture.Apply();
    }

    public WorldGenBlockNode[,] GetChunkNodeGrid(int chunkX, int chunkY, int chunkSize)
    {
        WorldGenBlockNode[,] data = new WorldGenBlockNode[chunkSize, chunkSize];
        
        for(int x = 0; x < chunkSize; x++)
            for (int y = 0; y < chunkSize; y++)
                data[x, y] = blockGrid[chunkX * chunkSize + x, chunkY * chunkSize + y];
        
        return data;
    }
}
