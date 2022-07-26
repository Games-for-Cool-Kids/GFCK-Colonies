using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JobType
{
    UNEMPLOYED, // :(
    COURIER,
    LUMBERJACK,
    MINER,
}


// ToDo: First time unit starts a job it should have a task to go the job building, to take a tool and start.
//       Then we should calculate a path between job site and building and share this between go-to-resource and go-to-building tasks.
//       So we don't have to recalculate path all the time.
public class Job
{
    public Building building { get; private set; }
    public Unit unit = null;

    public JobType type = JobType.UNEMPLOYED;
    public List<Task> tasks = new();
    private Task currentTask = null;

    private bool _forceStop = false;
    public bool repeat = true;

    public Job(Building building)
    {
        this.building = building; // All jobs are created by buildings.
    }

    public virtual void Start()
    {
        StartNextTask();
    }
    public virtual void Tick()
    {
        if (_forceStop)
        {
            Finish();
            return;
        }

        currentTask.Tick();
    }
    public virtual void Finish()
    {
        currentTask = null;
        unit.StopJob(); 
        JobManager.Instance.UnregisterJob(this);
    }

    public virtual void ForceStop()
    {
        _forceStop = true;
    }

    public void StartNextTask()
    {
        if (currentTask == null)
            currentTask = tasks[0];

        int i = tasks.IndexOf(currentTask);

        if (i == tasks.Count - 1)
        {
            if (repeat)
                currentTask = tasks[0];
            else
                Finish();
        }
        else
            currentTask = tasks[i + 1];

        currentTask.Start();
    }
}
