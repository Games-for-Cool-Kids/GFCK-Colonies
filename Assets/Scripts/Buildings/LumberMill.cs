using Jobs;

public class LumberMill : ResourceHarvestBuilding
{
    public LumberMill() : base(ResourceType.Wood)
    { }

    protected override void Start()
    {
        base.Start();

        AddJob(JobType.LUMBERJACK);
    }
}
