using UnityEngine;
using Jobs;

public class UnitComponentJob : UnitComponentBase
{
    private Transform _toolSlot = null;

    public Job job { get; private set; } = null;

    // TODO Too specific
    public int harvestDamage = 10;
    public float harvestSpeed = 1; // Amount of times per second.

    public float transferSpeed = 1; // Resources transferred per second.

    public int inventorySize = 5; // ToDo: Implement max inventory size inside Inventory class.

    protected override void Start()
    {
        base.Start();

        _toolSlot = transform.Find("Tool");
        Debug.Assert(_toolSlot != null);
    }

    void Update()
    {
        // TODO We should probably avoid polling. Let the JobManager look for an available Unit when a new job becomes available
        if (job == null)
        {
            ApplyForJob();
        }  
        else
        {
            job.Tick();
        }
    }

    public void ApplyForJob()
    {
        JobManager.Instance.AssignToAvailableJob(this);
    }

    public void ClearJob()
    {
        job = null;
    }

    public void FireFromJob()
    {
        JobManager.Instance.GiveJobBackToLaborMarket(job);
        job = null;
    }

    public void AssignJob(Job newJob)
    {
        job = newJob;

        var tool = JobManager.Instance.tools[job.type];
        if (tool != null)
            Instantiate(tool, _toolSlot);
    }
}
