using UnityEngine;

[CreateAssetMenu(menuName = "World/World Variable")]
public class WorldVariable : ScriptableObject
{
    public int size { get; private set; } // square size x size.
    public int height { get; private set; }

    public Texture2D texture { get; private set; }
    public Sprite worldSprite { get; private set; }

    public WorldGenBlockNode[,] grid;

    public void Init(int size, int height)
    {
        this.size = size;
        this.height = height;

        texture = new Texture2D(size, size);

        Rect rect = new Rect(0, 0, size, size);
        worldSprite = Sprite.Create(texture, rect, Vector2.zero, 100, 0, SpriteMeshType.FullRect);

        grid = new WorldGenBlockNode[size, size];
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                grid[x, y] = new(x, y);
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
                data[x, y] = grid[chunkX * chunkSize + x, chunkY * chunkSize + y];
        
        return data;
    }
}
