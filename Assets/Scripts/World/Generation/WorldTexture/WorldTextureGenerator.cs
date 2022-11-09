using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using World;

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

    public float NoiseScale = 1.0f;
    private Texture2D _noiseTex;

    // Block nodes
    public HeightMapStep heightMapStep;
    public HeightToBlockTypeStep heightToBlockStep;
    public CreateBeachesStep beachStep;

    // Surface stuff
    // Resource nodes
    public CreateResourcesStep ResourcesStep;

    public WorldVariable worldVariable;

    private const int _noSeed = -1;
    public int Seed = _noSeed;

    delegate void StepLogic(int x, int y);


    private void Start()
    {
        GenerateNoiseTexture();

        Generate();
    }

    private void Generate()
    {
        SetSeed();

        // Init
        worldVariable.Init(textureSize, maxHeight);
        InitImageInScene();

        // Height
        GenerateHeightMap();

        // Block types
        ScanWorld(ApplyBaseBlockTypeStep);
        SetWaterBlocksToCorrectLevel();
        ScanWorld(ApplyBeachStep);

        // Resources
        ScanWorld(ApplyResourcesStep);

        // Apply
        DrawWorld();

        RestoreEngineSeed();
    }

    private void GenerateNoiseTexture()
    {
        _noiseTex = new Texture2D(textureSize, textureSize);

        Color[] pixels = new Color[_noiseTex.width * _noiseTex.height];

        // For each pixel in the texture...
        float y = 0.0F;

        while (y < _noiseTex.height)
        {
            float x = 0.0F;
            while (x < _noiseTex.width)
            {
                float xCoord = x / _noiseTex.width * NoiseScale;
                float yCoord = y / _noiseTex.height * NoiseScale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pixels[(int)y * _noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        _noiseTex.SetPixels(pixels);
        _noiseTex.Apply();
    }

    private void InitImageInScene()
    { 
        imageObject.GetComponent<Image>().sprite = worldVariable.worldSprite;
    }

    private void ScanWorld(StepLogic logic)
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

    private void ApplyBaseBlockTypeStep(int x, int y)
    {
        WorldGenBlockNode node = worldVariable.blockGrid[x, y];
        node.type = heightToBlockStep.GetNodeType(node, worldVariable, textureSize, textureSize);
    }

    private void ApplyBeachStep(int x, int y)
    {
        WorldGenBlockNode node = worldVariable.blockGrid[x, y];
        node.type = beachStep.GetNodeType(node, worldVariable, textureSize, textureSize);
    }

    private void ApplyResourcesStep(int x, int y)
    {
        WorldGenBlockNode node = worldVariable.blockGrid[x, y];
        WorldGenResourceNode nodeResource = worldVariable.resourceGrid[x, y];

        nodeResource.type = ResourcesStep.GetResourceType(node, worldVariable, _noiseTex, textureSize, textureSize);
    }

    private void SetWaterBlocksToCorrectLevel()
    {
        float waterLevel = GetBlockHeightClosestTo(heightToBlockStep.waterLevel);

        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                WorldGenBlockNode node = worldVariable.blockGrid[x, y];

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
                WorldGenBlockNode node = worldVariable.blockGrid[x, y];
                WorldGenResourceNode nodeResource = worldVariable.resourceGrid[x, y];

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

                if(nodeResource.type == ResourceType.RESOURCE_WOOD)
                {
                    //pixel.r = 0.0f;
                    //pixel.b = 0.0f;
                    pixel.g = 1.0f;
                }

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

    private void SetSeed()
    {
        if (Seed != _noSeed)
        {
            UnityEngine.Random.InitState(Seed);
        }
    }

    private void RestoreEngineSeed()
    {
        if (Seed != _noSeed)
        {
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        }
    }
}
