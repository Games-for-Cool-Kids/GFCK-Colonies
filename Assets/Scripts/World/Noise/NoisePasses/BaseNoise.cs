using UnityEngine;
using SimplexNoise;

[CreateAssetMenu(menuName = "Noise/Base Noise")]
public class BaseNoise : NoiseBase
{
    public override int Calculate(ChunkStats stats, Vector3 noisePosition)
    {
        return GetNoise(noisePosition.x, 0, noisePosition.z, stats.baseNoise, Mathf.RoundToInt(stats.baseNoiseHeight));
    }
}
