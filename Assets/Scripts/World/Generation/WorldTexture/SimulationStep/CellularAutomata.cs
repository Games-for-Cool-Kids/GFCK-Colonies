using UnityEngine;

[CreateAssetMenu(menuName = "World/SimulationStep/Cellular Automata")]
public class CellularAutomata : SimulationStep
{
    public int death = 3;
    public int birth = 4;

    public override Block.Type GetNodeType(WorldTextureNode node, WorldTextureNode[,] grid, int maxX, int maxY)
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
                    if (neighbor.type != Block.Type.WATER)
                    {
                        groundNeighborCount++;
                    }
                }
                else
                {
                    return Block.Type.WATER;
                }
            }
        }

        if (node.type != Block.Type.WATER)
        {
            if (groundNeighborCount < death)
                return Block.Type.WATER;
            else
                return Block.Type.GRASS;
        }
        else
        {
            if (groundNeighborCount > birth)
                return Block.Type.GRASS;
            else
                return Block.Type.WATER;
        }
    }
}
