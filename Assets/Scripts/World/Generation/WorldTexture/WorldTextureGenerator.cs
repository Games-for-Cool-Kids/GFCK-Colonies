using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using World;

public class WorldTextureGenerator : MonoBehaviour
{
    private class NoiseTextureLayerData
    {
        public NoiseTextureLayerData(float scale, Vector2 offset, float selectionBias)
        {
            Scale = scale;
            Offset = offset;
            SelectionBias = selectionBias;
        }

        public float Scale { get; private set; }
        public Vector2 Offset { get; private set; }
        public float SelectionBias { get; private set; } // Between 0 to 1, when adding all biases of all layers together
    }

    public GameObject imageObject;

    public Color ground;
    public Color grass;
    public Color water;
    public Color sand;
    public Color snow;

    public int textureSize = 256; // Always a square.
    public int maxHeight = 50;

    private Texture2D _forestNoiseTex;
    private Texture2D _rocksNoiseTex;

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
        Generate();
    }

    private void Generate()
    {
        SetSeed();

        IEnumerable<NoiseTextureLayerData> forestLayers = new List<NoiseTextureLayerData>()
        { 
            new NoiseTextureLayerData(4.0f, new Vector2(UnityEngine.Random.Range(0, 4.0f), 
                                                        UnityEngine.Random.Range(0, 4.0f)), 
                                      0.5f),
            new NoiseTextureLayerData(15.0f, new Vector2(UnityEngine.Random.Range(0, 15.0f), 
                                                         UnityEngine.Random.Range(0, 15.0f)), 
                                      0.5f) 
        };

        _forestNoiseTex = GenerateLayeredNoiseTexture(textureSize, forestLayers);

        IEnumerable<NoiseTextureLayerData> rockLayers = new List<NoiseTextureLayerData>()
        {
            new NoiseTextureLayerData(4.0f, new Vector2(UnityEngine.Random.Range(0, 4.0f),
                                                        UnityEngine.Random.Range(0, 4.0f)),
                                      0.5f),
            new NoiseTextureLayerData(15.0f, new Vector2(UnityEngine.Random.Range(0, 15.0f),
                                                         UnityEngine.Random.Range(0, 15.0f)),
                                      0.5f)
        };

        _rocksNoiseTex = GenerateLayeredNoiseTexture(textureSize, rockLayers);

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

    // TODO Can store up to 3 different noise textures across RGB channels of a single texxture
    Texture2D GenerateLayeredNoiseTexture(int textureSize, IEnumerable<NoiseTextureLayerData> layersData)
    {
        Texture2D noiseTex = new Texture2D(textureSize, textureSize); // TODO Don't need to generate mip-maps

        Color[] pixels = new Color[textureSize * textureSize];

        foreach(var layerData in layersData)
        {
            float noiseScale = layerData.Scale;
            float offsetX = layerData.Offset.x;
            float offsetY = layerData.Offset.y;
            float bias = layerData.SelectionBias;

            float y = 0.0f;

            while (y < noiseTex.height)
            {
                float x = 0.0f;
                while (x < noiseTex.width)
                {
                    float xCoord = x / textureSize * noiseScale + offsetX;
                    float yCoord = y / textureSize * noiseScale + offsetY;
                    float sample = Mathf.PerlinNoise(xCoord, yCoord) * bias;

                    pixels[(int)y * textureSize + (int)x] += new Color(sample, sample, sample);
                    x++;
                }
                y++;
            }
        }

        noiseTex.SetPixels(pixels);
        noiseTex.Apply();

        return noiseTex;
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

        nodeResource.type = ResourcesStep.GetResourceType(node, worldVariable, _forestNoiseTex, textureSize, textureSize);
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

                if(nodeResource.type == ResourceType.Wood)
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
