﻿using UnityEngine;
using System.Collections.Generic;

public class JobFactory
{
    public static Job CreateJob(JobType type, Building jobBuilding)
    {
        switch (type)
        {
            case JobType.COURIER:
                return CreateCourierJob(jobBuilding);

            case JobType.LUMBERJACK:
                return CreateLumberJackJob(jobBuilding);

            case JobType.MINER:
                return CreateMinerJob(jobBuilding);

            case JobType.UNEMPLOYED:
            default:
                return CreateUnemployedJob(jobBuilding);
        }
    }

    private static Job CreateUnemployedJob(Building jobBuilding)
    {
        Debug.Log("Unemployed job not yet implemented");
        return new Job(jobBuilding);
    }
    private static Job CreateCourierJob(Building jobBuilding)
    {
        Debug.Log("Miner job not yet implemented");
        return new Job(jobBuilding);
    }
    private static Job CreateLumberJackJob(Building jobBuilding)
    {
        var job = new Job(jobBuilding);

        job.tasks.Add(new MoveToClosestTreeTask(job));
        job.tasks.Add(CreateMoveToJobBuildingTask(job));

        return job;
    }
    private static Job CreateMinerJob(Building jobBuilding)
    {
        Debug.Log("Miner job not yet implemented");
        return new Job(jobBuilding);
    }

    private static Task CreateMoveToJobBuildingTask(Job job)
    {
        var task = new MoveToObjectTask(job);
        task.targetObject = job.building.gameObject;
        return task;
    }
}