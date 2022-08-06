using UnityEngine;

[CreateAssetMenu(menuName = "World/SimulationStep/HeightToBlockType")]
public class HeightToBlockTypeStep : SimulationStep
{
    public float waterLevel = 0.2f;
    public float snowLevel = 0.8f;

    public override BlockType GetNodeType(WorldGenBlockNode node, WorldVariable worldVar, int maxX, int maxY)
    {
        float height = node.height;
        if (height <= waterLevel)
        {
            return BlockType.WATER;
        }
        else if(height >= snowLevel)
        {
            return BlockType.SNOW;
        }
        else
        {
            return BlockType.GROUND;
        }
    }
}
