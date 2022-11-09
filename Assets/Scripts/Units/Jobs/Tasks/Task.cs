using UnityEngine;

namespace Jobs
{
    public abstract class Task
    {
        protected Job job; // Parent job
        protected bool _forceStop = false;

        private TaskFlag _flags; // Remove task from job on finish.

        public delegate void TaskEvent();
        public event TaskEvent Finished;
         
        public Task(Job job, TaskFlag flags)
        {
            this.job = job;
            this._flags = flags;
        }

        public virtual void Start()
        {
            //Debug.Log("Starting task on unit: " + job.GetAssignedUnit().GetInstanceID()
            //    + ((_flags & TaskFlag.OneTime) == TaskFlag.OneTime ? ". This is a one-time task" : ""));
        }

        public virtual void Tick()
        {
            if (_forceStop)
            {
                Finish();
            }
        }
        public virtual void Finish()
        {
            Finished?.Invoke();
            //Debug.Log("Task is done.");

            if ((_flags & TaskFlag.OneTime) == TaskFlag.OneTime)
                job.tasks.Remove(this);
                //Debug.Log("Task was one-time; removing from job");
        }

        public virtual void ForceStop()
        {
            _forceStop = true;
        }

        public abstract string GetTaskDebugDescription();
    }
}
