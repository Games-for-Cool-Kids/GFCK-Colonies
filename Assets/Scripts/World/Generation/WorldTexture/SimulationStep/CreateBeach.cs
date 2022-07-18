using UnityEngine;

[CreateAssetMenu(menuName = "World/SimulationStep/Beach")]
public class CreateBeach : SimulationStep
{
    public override Block.Type GetNodeType(WorldTextureNode node, WorldTextureNode[,] grid, int maxX, int maxY)
    {
        // Only create a beach on normal ground nodes.
        if (node.type == Block.Type.WATER)
            return node.type;

        bool neighboringWater = false;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int _x = x + node.x;
                int _y = y + node.y;
                if (_x == node.x && _y == node.y)
                    continue;

                WorldTextureNode neighbor = GetNodeFromClone(_x, _y, grid, maxX, maxY);

                if (neighbor.type == Block.Type.WATER)
                {
                    neighboringWater = true;
                    goto ApplyResult;
                }
            }
        }

        ApplyResult: // from goto
        if (neighboringWater)
            return Block.Type.SAND;
        else
            return node.type;
    }
}

