using UnityEngine;
using World;

[CreateAssetMenu(menuName = "GameWorld/SimulationStep/Cellular Automata")]
public class CellularAutomata : SimulationStepBlockNode
{
    public int death = 3;
    public int birth = 4;

    public override BlockType GetNodeType(WorldGenBlockNode node, WorldVariable worldVar, int maxX, int maxY)
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

                WorldGenBlockNode neighbor = GetNodeFromGrid(_x, _y, worldVar.grid, maxX, maxY);
                if (neighbor != null)
                {
                    if (neighbor.type != BlockType.WATER)
                        groundNeighborCount++;
                }
                else
                    return BlockType.WATER;
            }
        }

        if (node.type != BlockType.WATER)
        {
            if (groundNeighborCount < death)
                return BlockType.WATER;
            else
                return BlockType.GRASS;
        }
        else
        {
            if (groundNeighborCount > birth)
                return BlockType.GRASS;
            else
                return BlockType.WATER;
        }
    }
}
