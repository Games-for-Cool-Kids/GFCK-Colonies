using Jobs;

public class StoneCutter : ResourceHarvestBuilding
{
    public StoneCutter() : base(ResourceType.Stone)
    { }

    protected override void Start()
    {
        base.Start();

        AddJob(JobType.MINER);
    }
}
