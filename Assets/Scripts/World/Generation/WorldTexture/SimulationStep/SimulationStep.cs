using UnityEngine;
using World;

public abstract class SimulationStep : ScriptableObject
{
	public abstract World.BlockType GetNodeType(WorldGenBlockNode node, WorldVariable worldVar, int maxX, int maxY);

	protected WorldGenBlockNode GetNodeFromGrid(int x, int y, WorldGenBlockNode[,] grid, int maxX, int maxY)
	{
		if (x < 0 || y < 0
		 || x >= maxX || y >= maxY)
			return null;

		return grid[x, y];
	}
}
