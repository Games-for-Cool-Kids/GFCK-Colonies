using UnityEngine;

[CreateAssetMenu(menuName = "World/SimulationStep/HeightToBlockType")]
public class HeightToBlockTypeStep : SimulationStep
{
    public float waterLevel = 0.2f;
    public float snowLevel = 0.8f;
    public int maxHeight = 50;

    public override Block.Type GetNodeType(WorldGenBlockNode node, WorldVariable worldVar, int maxX, int maxY)
    {
        float height = node.height;
        if (height <= waterLevel)
        {
            return Block.Type.WATER;
        }
        else if(height >= snowLevel)
        {
            return Block.Type.SNOW;
        }
        else
        {
            return Block.Type.GROUND;
        }
    }
}
