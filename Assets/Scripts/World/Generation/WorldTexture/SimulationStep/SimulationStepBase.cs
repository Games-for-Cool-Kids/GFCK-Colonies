using UnityEngine;

public abstract class SimulationStepBase : ScriptableObject
{
    protected WorldGenBlockNode GetNodeFromGrid(int x, int y, WorldGenBlockNode[,] grid, int maxX, int maxY)
    {
        if (x < 0 || y < 0
         || x >= maxX || y >= maxY)
            return null;

        return grid[x, y];
    }
}
