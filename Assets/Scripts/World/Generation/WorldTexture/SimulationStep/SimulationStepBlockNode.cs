public abstract class SimulationStepBlockNode : SimulationStepBase
{
	public abstract World.BlockType GetNodeType(WorldGenBlockNode node, WorldVariable worldVar, int maxX, int maxY);
}
