using Jobs;

public class LumberMill : ResourceHarvestBuilding
{
    public LumberMill() : base(ResourceType.Wood)
    { }

    void Start()
    {
        AddJob(JobType.LUMBERJACK);
    }
}
