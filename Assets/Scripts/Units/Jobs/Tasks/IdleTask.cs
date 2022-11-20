using UnityEngine;

namespace Jobs
{
    public class IdleTask : Task
    {
        public float IdleTime = 1.0f;
        private float _timeIdling = 0;

        public IdleTask(Job job, TaskFlag flags = TaskFlag.None) : base(job, flags)
        { }

        public override void Start()
        {
            base.Start();

        }

        public override void Tick()
        {
            base.Tick();

            _timeIdling += Time.deltaTime;
            if (_timeIdling >= IdleTime)
            {
                _timeIdling = 0;
                Finish();
            }
        }

        public override string GetTaskDescription()
        {
            return "Idling";
        }
    }
}
