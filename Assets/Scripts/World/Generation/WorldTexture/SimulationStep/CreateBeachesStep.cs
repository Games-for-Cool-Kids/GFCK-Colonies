using UnityEngine;
using World;

[CreateAssetMenu(menuName = "GameWorld/SimulationStep/Beach")]
public class CreateBeachesStep : SimulationStep
{
    public override BlockType GetNodeType(WorldGenBlockNode node, WorldVariable worldVar, int maxX, int maxY)
    {
        if (node.type == BlockType.WATER)
            return node.type;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int _x = x + node.x;
                int _y = y + node.y;
                if (_x == node.x && _y == node.y)
                    continue;

                WorldGenBlockNode neighbor = GetNodeFromGrid(_x, _y, worldVar.grid, maxX, maxY);
                if (neighbor == null)
                    continue;

                if (neighbor.type != BlockType.WATER)
                    continue;

                int nodeY = Mathf.FloorToInt(node.height * worldVar.height);
                int neighborY = Mathf.FloorToInt(neighbor.height * worldVar.height);

                if (nodeY - neighborY == 0)
                {
                    return BlockType.SAND;
                }
            }
        }

        return node.type;
    }
}

