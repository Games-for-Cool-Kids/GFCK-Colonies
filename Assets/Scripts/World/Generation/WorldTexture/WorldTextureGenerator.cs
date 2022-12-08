using System;
using System.Collections.Generic;
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

    public int TextureSize = 256; // Always a square.
    public int MaxHeight = 50;

    // Block nodes
    public HeightMapStep heightMapStep;
    public HeightToBlockTypeStep heightToBlockStep;
    public CreateBeachesStep beachStep;

    // Surface stuff
    // Resource nodes
    public CreateResourcesTreesStep ResourcesTreesStep;
    public CreateResourcesStoneStep ResourcesStoneStep;

    public WorldVariable worldVariable;

    private const int _noSeed = -1;
    public int Seed = _noSeed;

    delegate void StepLogic(int x, int y);


    private void Start()
    {
        Generate();
    }

    private void Generate()
    {
        SetSeed();

        // TODO Not a great solution.. This needs to be handled automatically for new steps. Awake()?
        ResourcesTreesStep.RegenerateNoiseMap(TextureSize);
        ResourcesStoneStep.RegenerateNoiseMap(TextureSize);

        // Init
        worldVariable.Init(TextureSize, MaxHeight);
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

    // TODO Can store up to 3 different noise textures across RGB channels of a single texxture

    private void InitImageInScene()
    { 
        imageObject.GetComponent<Image>().sprite = worldVariable.worldSprite;
    }

    private void ScanWorld(StepLogic logic)
    {
        for (int x = 0; x < TextureSize; x++)
        {
            for (int y = 0; y < TextureSize; y++)
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
        node.type = heightToBlockStep.GetNodeType(node, worldVariable, TextureSize, TextureSize);
    }

    private void ApplyBeachStep(int x, int y)
    {
        WorldGenBlockNode node = worldVariable.blockGrid[x, y];
        node.type = beachStep.GetNodeType(node, worldVariable, TextureSize, TextureSize);
    }

    private void ApplyResourcesStep(int x, int y)
    {
        WorldGenBlockNode node = worldVariable.blockGrid[x, y];
        WorldGenResourceNode nodeResource = worldVariable.resourceGrid[x, y];

        nodeResource.type = ResourcesTreesStep.GetResourceType(node, worldVariable);

        // TODO Temporary, but not ideal. If there are many trees, we have a smaller chance of getting rock.
        // Also, this is ugly.
        if(nodeResource.type == ResourceType.Invalid)
        {
            nodeResource.type = ResourcesStoneStep.GetResourceType(node, worldVariable);
        }
    }

    private void SetWaterBlocksToCorrectLevel()
    {
        float waterLevel = GetBlockHeightClosestTo(heightToBlockStep.waterLevel);

        for (int x = 0; x < TextureSize; x++)
        {
            for (int y = 0; y < TextureSize; y++)
            {
                WorldGenBlockNode node = worldVariable.blockGrid[x, y];

                if (node.type == BlockType.WATER)
                    node.height = waterLevel;
            }
        }
    }

    private void DrawWorld(bool drawHeight = false)
    {
        Texture2D heightMap = new Texture2D(TextureSize, TextureSize);

        for (int x = 0; x < TextureSize; x++)
        {
            for (int y = 0; y < TextureSize; y++)
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

                if(nodeResource.type == ResourceType.Wood)
                {
                    //pixel.r = 0.0f;
                    //pixel.b = 0.0f;
                    pixel.g = 1.0f;
                } else if(nodeResource.type == ResourceType.Stone)
                {
                    pixel.r = 1.0f;
                    //pixel.b = 0.0f;
                    //pixel.g = 1.0f;
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
