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

    public Color groundLow;
    public Color groundHigh;
    public Color waterDepth;
    public Color waterShallow;


    public int textureSize = 256; // Always a square.
    public int maxHeight = 50;
    public bool reverseHeight = false;

    public HeightMapStep heightMapStep;
    public SimulationStep baseStep;
    public SimulationStep beachStep;

    public WorldVariable worldVariable;

    delegate void StepLogic(int x, int y);


    private void Start()
    {
        worldVariable.Init(textureSize, maxHeight);

        InitImageInScene();

        CreateBase();
    }

    private void InitImageInScene()
    { 
        imageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(textureSize, textureSize);
        imageObject.GetComponent<Image>().sprite = worldVariable.worldSprite;
    }

    public void CreateBase()
    {
        GenerateHeightMap();

        ScanWorld(ApplyBaseStep);

        SetWaterBlocksToCorrectLevel();

        ScanWorld(ApplyBeachStep);

        DrawWorld();
    }

    //public void Step()
    //{
    //    //DrawWorld();
    //}

    //public void ApplySteps(int amount)
    //{
    //    for (int i = 0; i < amount; i++)
    //    {
    //        Step();
    //    }
    //}

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
        node.type = baseStep.GetNodeType(node, worldVariable, textureSize, textureSize);
    }

    private void ApplyBeachStep(int x, int y)
    {
        WorldGenBlockNode node = worldVariable.grid[x, y];
        node.type = beachStep.GetNodeType(node, worldVariable, textureSize, textureSize);
    }

    private void SetWaterBlocksToCorrectLevel()
    {
        float singleBlockHeight = 1.0f / worldVariable.height;
        float waterLevel = 0;
        for (int i = 0; waterLevel < 0.2f - singleBlockHeight; i++)
            waterLevel += singleBlockHeight;

        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                WorldGenBlockNode node = worldVariable.grid[x, y];

                if (node.type == Block.Type.WATER)
                {
                    node.height = waterLevel;
                }
            }
        }
    }

    private void DrawWorld(bool drawHeight = false)
    {
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                WorldGenBlockNode node = worldVariable.grid[x, y];

                Color pixel = Color.magenta;
                if(drawHeight)
                {
                    float height = worldVariable.grid[x, y].height;
                    pixel = GetHeightColor(height);
                }
                else
                {
                    switch (node.type)
                    {
                        case Block.Type.GROUND:
                            pixel = ground;
                            break;
                        case Block.Type.GRASS:
                            pixel = grass;
                            break;
                        case Block.Type.WATER:
                            pixel = water;
                            break;
                        case Block.Type.SAND:
                            pixel = sand;
                            break;
                        case Block.Type.SNOW:
                            pixel = snow;
                            break;
                    }
                }

                worldVariable.texture.SetPixel(x, y, pixel);
            }
        }

        worldVariable.ApplyTexture();
    }

    private Color GetHeightColor(float height)
    {
        return Color.magenta;
        //if (reverseHeight)
        //    height = 1 - height;
        //
        //if (height <= waterLevel)
        //{
        //    float level = height / waterLevel;
        //    return Color.Lerp(waterDepth, waterShallow, level);
        //}
        //else
        //{
        //    if (height >= snowLevel)
        //    {
        //        float level = (height - snowLevel) / (1 - snowLevel);
        //        return Color.Lerp(groundHigh, snow, level);
        //    }
        //    else
        //    {
        //        float level = (height - waterLevel) / (snowLevel - waterLevel);
        //        return Color.Lerp(groundLow, groundHigh, level);
        //    }
        //}
    }

    public void Load3DWorld()
    {
        SceneManager.LoadScene("WorldGen3D");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
