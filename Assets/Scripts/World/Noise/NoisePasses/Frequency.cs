using UnityEngine;
using SimplexNoise;

[CreateAssetMenu(menuName = "Noise/Frequency")]
public class Frequency : NoiseBase
{
    public override int Calculate(ChunkGeneratorStats stats, Vector3 noisePosition)
    {
        return GetNoise(noisePosition.x, 0, noisePosition.z, stats.frequency, stats.maxY);
    }
}
