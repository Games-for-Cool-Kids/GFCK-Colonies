using Jobs;

public class LumberMill : ResourceHarvestBuilding
{
    public LumberMill() : base(ResourceType.RESOURCE_WOOD)
    { }

    protected override void Start()
    {
        base.Start();

        AddJob(JobType.LUMBERJACK);
    }
}
