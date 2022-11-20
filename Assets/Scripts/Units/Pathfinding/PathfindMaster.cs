using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using World;

namespace Pathfinding
{
    //This class controls the pathfinding threads
    public class PathfindMaster : MonoBehaviourSingleton<PathfindMaster>
    {
        //The maximum simultaneous threads we allow to open
        public int MaxThreads = 3;

        //Delegates are a variable that points to a function
        public delegate void PathFindingThreadComplete(List<Block> path);

        private List<Pathfinder> currentThreads;
        private List<Pathfinder> todoThreads;

        private PathFinderCache _cache;

        void Start()
        {
            currentThreads = new List<Pathfinder>();
            todoThreads = new List<Pathfinder>();

            _cache = new PathFinderCache(GameManager.Instance.World);
        }
   
        void Update() 
        {
            int i = 0;

            while(i < currentThreads.Count)
            {
                if(currentThreads[i].executionFinished)
                {
                    currentThreads[i].NotifyComplete();
                    currentThreads.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            if(todoThreads.Count > 0 && currentThreads.Count < MaxThreads)
            {
                Pathfinder thread = todoThreads[0];
                todoThreads.RemoveAt(0);
                currentThreads.Add(thread);

                //Start a new thread
                Thread threadThread = new Thread(thread.FindPath);
                threadThread.Start();

                //As per the doc
                //https://msdn.microsoft.com/en-us/library/system.threading.thread(v=vs.110).aspx
                //It is not necessary to retain a reference to a Thread object once you have started the thread. 
                //The thread continues to execute until the thread procedure is complete.				
            }
        }

        public void RequestPathfind(Block start, Block target, PathFindingThreadComplete completeCallback)
        {
            Debug.Assert(start != null);
            Debug.Assert(target != null);

            Pathfinder newThread = new Pathfinder(_cache, GameManager.Instance.World, start, target, completeCallback);
            todoThreads.Add(newThread);
        }
    }
}
