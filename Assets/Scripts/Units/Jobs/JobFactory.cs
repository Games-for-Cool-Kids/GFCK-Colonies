using UnityEngine;
using System.Collections.Generic;

namespace Jobs
{
    public class JobFactory
    {
        public static Job CreateJob(JobType type, Building jobBuilding)
        {
            switch (type)
            {
                case JobType.Courier:
                    return CreateCourierJob(jobBuilding);

                case JobType.Lumberjack:
                    return CreateLumberJackJob(jobBuilding);

                case JobType.Miner:
                    return CreateMinerJob(jobBuilding);

                case JobType.Unemployed:
                default:
                    return CreateUnemployedJob(jobBuilding);
            }
        }

        private static Job CreateUnemployedJob(Building jobBuilding)
        {
            Debug.Log("Unemployed job not yet implemented");
            return new Job(jobBuilding, JobType.Unemployed);
        }
        private static Job CreateCourierJob(Building jobBuilding)
        {
            var job = new Job(jobBuilding, JobType.Courier);

            var transferAllTask = new DeliverAllResourcesTask(job);
            transferAllTask.targetStorage = jobBuilding;

            job.tasks.Add(new IdleTask(job)); // Needed to avoid infinite loop when there are no transfer requests.
            job.tasks.Add(CreateMoveToJobBuildingTask(job));
            job.tasks.Add(transferAllTask); // Empty inventory whenever we return to stockpile.
            job.tasks.Add(new LookForResourceTransferRequestTask(job));

            return job;
        }
        private static Job CreateLumberJackJob(Building jobBuilding)
        {
            var job = new Job(jobBuilding, JobType.Lumberjack);

            var harvestTask = new HarvestResourceTask(job, ResourceType.Wood);
            var transferTask = new TransferResourcesTask(job, TransferType.Delivery);
            transferTask.targetStorage = jobBuilding;
            
            harvestTask.Finished += () => transferTask.Add(ResourceType.Wood, 1); // Tell transfer task to transfer the harvested resource(s).

            job.tasks.Add(new MoveToClosestTreeTask(job));
            job.tasks.Add(harvestTask);
            job.tasks.Add(CreateMoveToJobBuildingTask(job));
            job.tasks.Add(transferTask);

            return job;
        }
        private static Job CreateMinerJob(Building jobBuilding)
        {
            var job = new Job(jobBuilding, JobType.Miner);

            var harvestTask = new HarvestResourceTask(job, ResourceType.Stone);
            var transferTask = new TransferResourcesTask(job, TransferType.Delivery);
            transferTask.targetStorage = jobBuilding;

            harvestTask.Finished += () => transferTask.Add(ResourceType.Stone, 1); // Tell transfer task to transfer the harvested resource(s).

            job.tasks.Add(new MoveToClosestRockTask(job));
            job.tasks.Add(harvestTask);
            job.tasks.Add(CreateMoveToJobBuildingTask(job));
            job.tasks.Add(transferTask);

            return job;
        }

        private static Task CreateMoveToJobBuildingTask(Job job)
        {
            var task = new MoveToObjectTask(job);
            task.TargetObject = job.building.gameObject;
            return task;
        }
    }
}
