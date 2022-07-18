using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldTextureGenerator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public Color ground;
    public Color grass;
    public Color water;
    public Color sand;

    public WorldTextureNode[,] grid;
    public int textureSize = 256; // Always a square.

    public int initialWaterPercentage = 55;

    public SimulationStep currentStep;
    public SimulationStep beachStep;

    public WorldVariable worldVariable;

    private void Start()
    {
        worldVariable.Init(textureSize);
        spriteRenderer.sprite = worldVariable.worldSprite;

        CreateBase();
    }

    public void CreateBase()
    {
        grid = new WorldTextureNode[textureSize, textureSize];

        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                WorldTextureNode node = new();
                node.x = x;
                node.y = y;
                grid[x, y] = node;

                if (Random.Range(0, 100) > initialWaterPercentage)
                {
                    node.type = Block.Type.GRASS;
                }
                else
                {
                    node.type = Block.Type.WATER;
                }
            }
        }

        DrawWorld();
    }

    public void Step()
    {
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                WorldTextureNode node = grid[x, y];
                node.type = currentStep.GetNodeType(node, grid, textureSize, textureSize);
            }
        }

        ApplyBeachStep();

        DrawWorld();
    }

    public void ApplySteps(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Step();
        }
    }

    private void ApplyBeachStep()
    {
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                WorldTextureNode node = grid[x, y];
                node.type = beachStep.GetNodeType(node, grid, textureSize, textureSize);
            }
        }
    }

    private void DrawWorld()
    {
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                WorldTextureNode node = grid[x, y];

                switch (node.type)
                {
                    case Block.Type.GROUND:
                        worldVariable.texture.SetPixel(x, y, ground);
                        worldVariable.blockMap[x, y] = Block.Type.GROUND;
                        worldVariable.heightMap[x, y] = 2;
                        break;
                    case Block.Type.GRASS:
                        worldVariable.texture.SetPixel(x, y, grass);
                        worldVariable.blockMap[x, y] = Block.Type.GRASS;
                        worldVariable.heightMap[x, y] = 2;
                        break;
                    case Block.Type.WATER:
                        worldVariable.texture.SetPixel(x, y, water);
                        worldVariable.blockMap[x, y] = Block.Type.WATER;
                        worldVariable.heightMap[x, y] = 0;
                        break;
                    case Block.Type.SAND:
                        worldVariable.texture.SetPixel(x, y, sand);
                        worldVariable.blockMap[x, y] = Block.Type.SAND;
                        worldVariable.heightMap[x, y] = 1;
                        break;
                }
            }
        }

        worldVariable.ApplyTexture();
    }

    public void Load3DWorld()
    {
        SceneManager.LoadScene("WorldGenerationTestScene");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
