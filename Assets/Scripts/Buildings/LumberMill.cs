using Jobs;

public class LumberMill : Building
{
    void Start()
    {
        AddJob(JobType.LUMBERJACK);
    }
}
