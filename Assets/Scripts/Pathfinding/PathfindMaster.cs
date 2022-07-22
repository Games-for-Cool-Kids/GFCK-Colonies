using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace Pathfinding
{
    //This class controls the pathfinding threads
    public class PathfindMaster : MonoBehaviourSingleton<PathfindMaster>
    {
        //The maximum simultaneous threads we allow to open
        public int MaxJobs = 3;

        //Delegates are a variable that points to a function
        public delegate void PathFindingJobComplete(List<BlockData> path);

        private List<Pathfinder> currentJobs;
        private List<Pathfinder> todoJobs;

        void Start()
        {
            currentJobs = new List<Pathfinder>();
            todoJobs = new List<Pathfinder>();
        }
   
        void Update() 
        {
            int i = 0;

            while(i < currentJobs.Count)
            {
                if(currentJobs[i].executionFinished)
                {
                    currentJobs[i].NotifyComplete();
                    currentJobs.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            if(todoJobs.Count > 0 && currentJobs.Count < MaxJobs)
            {
                Pathfinder job = todoJobs[0];
                todoJobs.RemoveAt(0);
                currentJobs.Add(job);

                //Start a new thread
                Thread jobThread = new Thread(job.FindPath);
                jobThread.Start();

                //As per the doc
                //https://msdn.microsoft.com/en-us/library/system.threading.thread(v=vs.110).aspx
                //It is not necessary to retain a reference to a Thread object once you have started the thread. 
                //The thread continues to execute until the thread procedure is complete.				
            }
        }

        public void RequestPathfind(BlockData start, BlockData target, PathFindingJobComplete completeCallback)
        {
            Pathfinder newJob = new Pathfinder(GameManager.Instance.World, start, target, completeCallback);
            todoJobs.Add(newJob);
        }
    }
}
