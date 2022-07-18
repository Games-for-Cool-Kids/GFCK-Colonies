using UnityEngine;

public abstract class SimulationStep : ScriptableObject
{
	public abstract bool isAlive(WorldTextureNode node, WorldTextureNode[,] cloneGrid, int maxX, int maxY);

	protected WorldTextureNode GetNodeFromClone(int x, int y, WorldTextureNode[,] cloneGrid, int maxX, int maxY)
	{
		if (x < 0 || y < 0
		 || x >= maxX || y >= maxY)
			return null;

		return cloneGrid[x, y];
	}
}
