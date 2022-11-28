
using UnityEngine;

public abstract class SimulationStepResourceNode : SimulationStepBase
{
    public NoiseMapGenerator NoiseMapGenerator;

    protected Texture2D _noiseTexture;

    public void RegenerateNoiseMap(int textureSize)
    {
        _noiseTexture = NoiseMapGenerator.GenerateLayeredNoiseTexture(textureSize);
    }

    public abstract ResourceType GetResourceType(WorldGenBlockNode node, WorldVariable worldVar);
}
