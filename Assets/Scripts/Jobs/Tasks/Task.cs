using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task
{
    protected Job job; // Parent job
    protected bool _forceStop = false;

    public delegate void TaskEvent();
    public event TaskEvent Finished;

    public Task(Job job)
    {
        this.job = job;
    }

    public virtual void Start()
    {

    }
    public virtual void Tick()
    {
        if (_forceStop)
        {
            Finish();
            return;
        }
    }
    public virtual void Finish()
    {
        Finished?.Invoke();
    }

    public virtual void ForceStop()
    {
        _forceStop = true;
    }
}
