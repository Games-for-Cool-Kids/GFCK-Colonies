using Jobs;

public class StoneCutter : ResourceHarvestBuilding
{
    public StoneCutter() : base(ResourceType.Stone)
    { }

    void Start()
    {
        AddJob(JobType.Miner);
    }
}
