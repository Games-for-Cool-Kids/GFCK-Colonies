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
    public int TextureSize; // TODO We should probably fetch this from somewhere, to make sure it can't be used at a "wrong" size

    public List<NoiseTextureLayerData> LayersGenerationData = new List<NoiseTextureLayerData>();

    // TODO Could store up to 3 different noise textures across RGB channels of a single texxture
    public Texture2D GenerateLayeredNoiseTexture()
    {
        Texture2D noiseTex = new Texture2D(TextureSize, TextureSize); // TODO Don't need to generate mip-maps

        Color[] pixels = new Color[TextureSize * TextureSize];

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
                    float xCoord = x / TextureSize * noiseScale + offsetX;
                    float yCoord = y / TextureSize * noiseScale + offsetY;
                    float sample = Mathf.PerlinNoise(xCoord, yCoord) * bias;

                    pixels[(int)y * TextureSize + (int)x] += new Color(sample, sample, sample);
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
