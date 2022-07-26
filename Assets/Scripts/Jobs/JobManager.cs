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

    public void RegisterJob(Job job)
    {
        availableJobs.Add(job);
    }

    public void UnregisterJob(Job job)
    {
        availableJobs.Remove(job);

        if (takenJobs.Remove(job))
            job.unit.ClearJob();
    }

    public Job AssignToAvailableJob(Unit employee)
    {
        if (availableJobs.Count == 0)
            return null;

        Job job = availableJobs[0];
        SetJobTaken(job);

        employee.job = job;
        job.unit = employee;

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
