using UnityEngine;

[CreateAssetMenu(menuName = "SimulationStep/Cellular Automata")]
public class CellularAutomata : SimulationStep
{
    public int death = 3;
    public int birth = 4;

    public override WorldTextureNode.Type GetNodeState(WorldTextureNode node, WorldTextureNode[,] grid, int maxX, int maxY)
    {
        int groundNeighborCount = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int _x = x + node.x;
                int _y = y + node.y;
                if (_x == node.x && _y == node.y)
                    continue;

                WorldTextureNode neighbor = GetNodeFromClone(_x, _y, grid, maxX, maxY);
                if (neighbor != null)
                {
                    if (neighbor.type != WorldTextureNode.Type.WATER)
                    {
                        groundNeighborCount++;
                    }
                }
                else
                {
                    return WorldTextureNode.Type.WATER;
                }
            }
        }

        if (node.type != WorldTextureNode.Type.WATER)
        {
            if (groundNeighborCount < death)
                return WorldTextureNode.Type.WATER;
            else
                return WorldTextureNode.Type.GRASS;
        }
        else
        {
            if (groundNeighborCount > birth)
                return WorldTextureNode.Type.GRASS;
            else
                return WorldTextureNode.Type.WATER;
        }
    }
}
