using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitComponentJob : BaseUnitComponent
{
    private Transform _toolSlot = null;

    protected Job job = null;

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
        // TODO We should probably avoid polling. Let the JobManager look for an available Unit when a new job becomes available
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
