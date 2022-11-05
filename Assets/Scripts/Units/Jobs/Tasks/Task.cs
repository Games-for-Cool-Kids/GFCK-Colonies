namespace Jobs
{
    public class Task
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

            if ((_flags & TaskFlag.OneTime) == TaskFlag.OneTime)
                job.tasks.Remove(this);
        }

        public virtual void ForceStop()
        {
            _forceStop = true;
        }
    }
}
