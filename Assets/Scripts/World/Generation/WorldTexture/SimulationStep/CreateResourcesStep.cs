using UnityEngine;
using World;

[CreateAssetMenu(menuName = "GameWorld/SimulationStep/ResourceNodes")]
public class CreateResourcesStep : SimulationStepResourceNode
{
    public float ClusterSize = 0.5f; // Between 0 and 1
    public float SecondaryClusterSize = 0.2f; // Between 0 and ClusterSize
    public float Scarceness = 0.25f; // Between 0 and 1
    

    public override ResourceType GetResourceType(WorldGenBlockNode node, WorldVariable worldVar, Texture2D noiseTexture, int maxX, int maxY)
    {
        if (node.type == BlockType.GROUND )
        {
            float noiseSample = noiseTexture.GetPixel(node.x, node.y).grayscale;
            if(noiseSample >= ClusterSize)
            {
                if (Random.Range(0, 2.0f) <= noiseSample - Scarceness)
                {
                    return ResourceType.Wood;
                }   
            } else if(noiseSample >= SecondaryClusterSize)
            {
                if (Random.Range(0, 5.0f) <= noiseSample - Scarceness)
                {
                    return ResourceType.Wood;
                }
            }
        }

        return ResourceType.Invalid;
    }
}
