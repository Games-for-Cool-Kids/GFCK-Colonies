using Jobs;

public class Stockpile : Building
{
    protected virtual void Start()
    {
        AddJob(JobType.Courier);
    }
}
