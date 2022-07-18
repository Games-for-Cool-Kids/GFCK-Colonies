using UnityEngine;

[CreateAssetMenu(menuName = "SimulationStep/Cellular Automata")]
public class CellularAutomata : SimulationStep
{
    public int death = 3;
    public int birth = 4;

    public override bool isAlive(WorldTextureNode node, WorldTextureNode[,] cloneGrid, int maxX, int maxY)
    {
        int neighborCount = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int _x = x + node.x;
                int _y = y + node.y;
                if (_x == node.x && _y == node.y)
                    continue;

                WorldTextureNode neighbor = GetNodeFromClone(_x, _y, cloneGrid, maxX, maxY);
                if (neighbor != null)
                {
                    if (neighbor.isGround)
                    {
                        neighborCount++;
                    }
                }
            }
        }

        if (node.isGround)
        {
            return neighborCount >= death;
        }
        else
        {
            return neighborCount > birth;
        }
    }
}
