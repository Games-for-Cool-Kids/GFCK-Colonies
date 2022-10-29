using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Buildings: Create jobs and gives to jobmanager.
// Jobmanager: Holds jobs.
// Units: Take jobs from jobmanager.

public class JobManager : MonoBehaviourSingleton<JobManager>
{
    private List<Job> availableJobs = new();
    private List<Job> takenJobs = new();

    public UDictionary<JobType, GameObject> tools;

    public void RegisterJob(Job job)
    {
        availableJobs.Add(job);
    }

    public void UnregisterJob(Job job)
    {
        availableJobs.Remove(job);

        if (takenJobs.Remove(job))
            job.UnitJobComponent.ClearJob();
    }

    // Bit of a weird thing. This class hold the job, but then passes ownership to the unit. This class could either
    // 1. Hold just JobData, and the unit (JobComponent) could start and maintain the job, or (better)
    // 2. This class keeps the ownership over the job and handles creation, destruction and ticking
    public Job AssignToAvailableJob(UnitComponentJob employee)
    {
        if (availableJobs.Count == 0)
            return null;

        Job job = availableJobs[0];
        job.UnitJobComponent = employee;

        SetJobTaken(job);
        employee.AssignJob(job);

        job.Start();

        return job;
    }

    private void SetJobTaken(Job job)
    {
        availableJobs.RemoveAt(0);
        takenJobs.Add(job);
    }

    public void GiveJobBackToLaborMarket(Job job)
    {
        takenJobs.Remove(job);
        if(!availableJobs.Contains(job))
            availableJobs.Add(job);
    }
}
