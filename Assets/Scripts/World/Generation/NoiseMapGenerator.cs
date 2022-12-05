using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NoiseTextureLayerData
{
    public float Scale;

    public Vector2 Offset;

    public float SelectionBias; // Between 0 to 1, when adding all biases of all layers together

    public NoiseTextureLayerData(float scale, Vector2 offset, float selectionBias)
    {
        Scale = scale;
        Offset = offset;
        SelectionBias = selectionBias;
    }
}

[CreateAssetMenu(menuName = "GameWorld/LayeredNoiseMap")]
public class NoiseMapGenerator : ScriptableObject
{
    public List<NoiseTextureLayerData> LayersGenerationData = new List<NoiseTextureLayerData>();

    // TODO Could store up to 3 different noise textures across RGB channels of a single texture
    public Texture2D GenerateLayeredNoiseTexture(int textureSize)
    {
        Texture2D noiseTex = new Texture2D(textureSize, textureSize, TextureFormat.RFloat, false); // single-channel, 32 bit float. 

        Color[] pixels = new Color[textureSize * textureSize];

        foreach (var layerData in LayersGenerationData)
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

                    pixels[(int)y * textureSize + (int)x] += new Color(sample, 0.0f, 0.0f);
                    x++;
                }
                y++;
            }
        }

        noiseTex.SetPixels(pixels);
        noiseTex.Apply();

        return noiseTex;
    }
}
