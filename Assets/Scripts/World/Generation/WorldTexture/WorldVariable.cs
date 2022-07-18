using UnityEngine;

[CreateAssetMenu(menuName = "World/World Variable")]
public class WorldVariable : ScriptableObject
{
    public int size { get; private set; } // square size x size.
    public Texture2D texture { get; private set; }

    public Sprite worldSprite { get; private set; }
    
    public Block.Type[,] blockMap { get; private set; }
    public int[,] heightMap { get; private set; }

    public void Init(int size)
    {
        this.size = size;
        texture = new Texture2D(size, size);

        Rect rect = new Rect(0, 0, size, size);
        worldSprite = Sprite.Create(texture, rect, Vector2.zero, 100, 0, SpriteMeshType.FullRect);

        blockMap = new Block.Type[size, size];
        heightMap = new int[size, size];
    }

    public void ApplyTexture()
    {
        texture.Apply();
    }

    public int[,] GetChunkHeightMap(int chunkX, int chunkY, int chunkSize)
    {
        int[,] data = new int[chunkSize, chunkSize];

        for(int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                data[x, y] = heightMap[chunkX * chunkSize + x, chunkY * chunkSize + y];
            }
        }

        return data;
    }

    public Block.Type[,] GetChunkBlockMap(int chunkX, int chunkY, int chunkSize)
    {
        Block.Type[,] data = new Block.Type[chunkSize, chunkSize];

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                data[x, y] = blockMap[chunkX * chunkSize + x, chunkY * chunkSize + y];
            }
        }

        return data;
    }
}
