
using UnityEngine;

public abstract class SimulationStepResourceNode : SimulationStepBase
{
    public abstract ResourceType GetResourceType(WorldGenBlockNode node, WorldVariable worldVar, Texture2D noiseTexture, int maxX, int maxY);
}
