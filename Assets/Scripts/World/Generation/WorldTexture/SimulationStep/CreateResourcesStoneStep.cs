using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World;

[CreateAssetMenu(menuName = "GameWorld/SimulationStep/ResourceNodesStones")]
public class CreateResourcesStoneStep : SimulationStepResourceNode
{
    public float ClusterSize = 0.5f; // Between 0 and 1
    public float Scarceness = 0.25f; // Between 0 and 1

    public override ResourceType GetResourceType(WorldGenBlockNode node, WorldVariable worldVar)
    {
        if (node.type == BlockType.GROUND)
        {
            float noiseSample = _noiseTexture.GetPixel(node.x, node.y).grayscale;
            if (noiseSample >= ClusterSize)
            {
                if (Random.Range(0, 2.0f) <= noiseSample - Scarceness)
                {
                    return ResourceType.Stone;
                }
            }
        }

        return ResourceType.Invalid;
    }
}
