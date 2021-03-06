using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldTextureGenerator : MonoBehaviour
{
    public GameObject imageObject;

    public Color ground;
    public Color grass;
    public Color water;
    public Color sand;
    public Color snow;

    public int textureSize = 256; // Always a square.
    public int maxHeight = 50;

    public HeightMapStep heightMapStep;
    public HeightToBlockTypeStep heightToBlockStep;
    public SimulationStep beachStep;

    public WorldVariable worldVariable;

    delegate void StepLogic(int x, int y);


    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        worldVariable.Init(textureSize, maxHeight);

        InitImageInScene();

        GenerateHeightMap();

        ScanWorld(ApplyBaseStep);

        SetWaterBlocksToCorrectLevel();

        ScanWorld(ApplyBeachStep);

        DrawWorld();
    }

    private void InitImageInScene()
    { 
        imageObject.GetComponent<Image>().sprite = worldVariable.worldSprite;
    }

    void ScanWorld(StepLogic logic)
    {
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                logic(x, y);
            }
        }
    }

    private void GenerateHeightMap()
    {
        heightMapStep.ApplyTo(worldVariable);
    }

    private void ApplyBaseStep(int x, int y)
    {
        WorldGenBlockNode node = worldVariable.grid[x, y];
        node.type = heightToBlockStep.GetNodeType(node, worldVariable, textureSize, textureSize);
    }

    private void ApplyBeachStep(int x, int y)
    {
        WorldGenBlockNode node = worldVariable.grid[x, y];
        node.type = beachStep.GetNodeType(node, worldVariable, textureSize, textureSize);
    }

    private void SetWaterBlocksToCorrectLevel()
    {
        float waterLevel = GetBlockHeightClosestTo(heightToBlockStep.waterLevel);

        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                WorldGenBlockNode node = worldVariable.grid[x, y];

                if (node.type == BlockType.WATER)
                    node.height = waterLevel;
            }
        }
    }

    private void DrawWorld(bool drawHeight = false)
    {
        Texture2D heightMap = new Texture2D(textureSize, textureSize);

        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                WorldGenBlockNode node = worldVariable.grid[x, y];

                Color pixel = Color.magenta;
                switch (node.type)
                {
                    case BlockType.GROUND:
                        pixel = ground;
                        break;
                    case BlockType.GRASS:
                        pixel = grass;
                        break;
                    case BlockType.WATER:
                        pixel = water;
                        break;
                    case BlockType.SAND:
                        pixel = sand;
                        break;
                    case BlockType.SNOW:
                        pixel = snow;
                        break;
                }
                pixel *= GetHeightColor(node.height);
                worldVariable.texture.SetPixel(x, y, pixel);
            }
        }

        heightMap.Apply();
        worldVariable.ApplyTexture();
    }

    private Color GetHeightColor(float height)
    {
        float _colorHeight = heightToBlockStep.waterLevel + height * 0.8f;

        Color heightColor = Color.white;
        if (height > heightToBlockStep.waterLevel)
        {
            heightColor = Color.Lerp(new Color(0.2f, 0.2f, 0.2f), Color.white, _colorHeight);
        }

        return heightColor;
    }

    public void Load3DWorld()
    {
        SceneManager.LoadScene("WorldGen3D");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    private float GetBlockHeightClosestTo(float height)
    {
        float h = height * worldVariable.height;
        h = Mathf.Floor(h);
        return h / worldVariable.height;
    }
}
