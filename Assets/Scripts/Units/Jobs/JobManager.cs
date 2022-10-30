using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Buildings: Create jobs and gives to jobmanager.
// Jobmanager: Holds jobs.
// Units: Take jobs from jobmanager.

namespace Jobs
{
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
            if (!availableJobs.Contains(job))
                availableJobs.Add(job);
        }
    }
}
