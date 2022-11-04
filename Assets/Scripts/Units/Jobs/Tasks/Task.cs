using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jobs
{
    public class Task
    {
        protected Job job; // Parent job
        protected bool _forceStop = false;

        private bool _oneTime; // Remove task from job on finish.

        public delegate void TaskEvent();
        public event TaskEvent Finished;

        public Task(Job job, bool oneTime)
        {
            this.job = job;
            _oneTime = oneTime;
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

            if (_oneTime)
                job.tasks.Remove(this);
        }

        public virtual void ForceStop()
        {
            _forceStop = true;
        }
    }
}
