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

        private bool _running = false; // Mostly for debug purposes, to check that start and finish are called correctly by derived implementations.
         
        public Task(Job job, TaskFlag flags)
        {
            this.job = job;
            this._flags = flags;
        }

        public virtual void Start()
        {
            _running = true;
        }

        public virtual void Tick()
        {
            Debug.Assert(_running);

            if (_forceStop)
            {
                Finish();
            }
        }
        public virtual void Finish()
        {
            Debug.Assert(_running);

            if ((_flags & TaskFlag.OneTime) == TaskFlag.OneTime)
                job.RemoveTask(this);
            
            Finished?.Invoke();

            _running = false;
        }

        public virtual void ForceStop()
        {
            _forceStop = true;
            _running = false;
        }

        public abstract string GetTaskDescription();
    }
}
