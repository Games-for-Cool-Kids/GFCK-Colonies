using UnityEngine;
using Jobs;

public class UnitComponentJob : BaseUnitComponent
{
    private Transform _toolSlot = null;

    public Job job = null;

    // TODO Too specific
    public int harvestDamage = 10;
    public float harvestSpeed = 1; // Amount of times per second.

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _toolSlot = transform.Find("Tool");
        Debug.Assert(_toolSlot != null);
    }

    // Update is called once per frame
    void Update()
    {
        if (job == null)
            ApplyForJob();
        else
            job.Tick();
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
