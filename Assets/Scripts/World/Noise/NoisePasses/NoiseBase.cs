using UnityEngine;
using SimplexNoise;

public abstract class NoiseBase : ScriptableObject
{
    public abstract int Calculate(ChunkGeneratorStats stats, Vector3 noisePosition);

    public int GetNoise(float x, float y, float z, float scale, int max)
    {
        return Mathf.FloorToInt((Noise.Generate(x * scale, y * scale, z * scale) + 1) * (max / 2.0f));
    }
}
