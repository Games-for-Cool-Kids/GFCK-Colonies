
using UnityEngine;

public abstract class SimulationStepResourceNode : SimulationStepBase
{
    public NoiseMapGenerator NoiseMapGenerator;

    protected Texture2D _noiseTexture;

    public void RegenerateNoiseMap()
    {
        _noiseTexture = NoiseMapGenerator.GenerateLayeredNoiseTexture();
    }

    public abstract ResourceType GetResourceType(WorldGenBlockNode node, WorldVariable worldVar);
}
