using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Jobs
{
    // ToDo: First time unit starts a job it should have a task to go the job building, to take a tool and start.
    //       Then we should calculate a path between job site and building and share this between go-to-resource and go-to-building tasks.
    //       So we don't have to recalculate path all the time.
    public class Job
    {
        public Building building { get; private set; }
        public UnitComponentJob UnitJobComponent = null;

        public JobType type = JobType.UNEMPLOYED;
        public List<Task> tasks = new();
        private Task currentTask = null;

        private bool _forceStop = false;
        public bool repeat = true;

        public Job(Building building, JobType type)
        {
            this.building = building; // All jobs are created by buildings.
            this.type = type;
        }

        public virtual void Start()
        {
            foreach (var task in tasks)
            {
                task.Finished += StartNextTask;
            }

            currentTask = tasks[0];
            currentTask.Start();
        }

        public void AddTask(Task newTask, int index = -1)
        {
            newTask.Finished += StartNextTask;

            if (index == -1) // -1 to add at end.
                tasks.Add(newTask);
            else
                tasks.Insert(index, newTask);
        }

        public void RemoveTask(Task task)
        {
            if(currentTask == task)
                StartNextTask();

            task.Finished -= StartNextTask;
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

            JobManager.Instance.UnregisterJob(this); // Also removes job from unit.
        }

        public virtual void ForceStop()
        {
            _forceStop = true;
        }

        public void StartNextTask()
        {
            int i = tasks.IndexOf(currentTask);
            if (i == tasks.Count - 1)
            {
                if (repeat)
                    currentTask = tasks[0];
                else
                    Finish();
            }
            else
            {
                currentTask = tasks[i + 1];
            }

            currentTask.Start();
        }

        public Unit GetAssignedUnit()
        {
            if (UnitJobComponent == null)
                return null;

            return UnitJobComponent.Owner;
        }

        public string GetCurrentTaskDescription()
        {
            return currentTask.GetTaskDescription();
        }
    }
}
