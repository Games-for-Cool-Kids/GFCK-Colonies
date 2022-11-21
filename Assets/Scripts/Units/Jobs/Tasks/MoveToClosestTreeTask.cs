using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jobs
{
    public class MoveToClosestTreeTask : MoveToObjectTask
    {
        public MoveToClosestTreeTask(Job job) : base(job) { }

        public override void Start()
        {
            TargetObject = FindTreeNearestToBuilding();

            GameManager.GameObjectCreated += UpdateTargetTreeIfCloser;

            base.Start();
        }

        public override void Finish()
        {
            GameManager.GameObjectCreated -= UpdateTargetTreeIfCloser;

            base.Finish();
        }

        // TODO Needs fixing! When building a new tree, GameObjectCreated() is called for the blueprint, firing this function on a dangerously invalid tree object!
        public void UpdateTargetTreeIfCloser(GameObject gameObject)
        {
            // ToDo: We should look for the closest tree that has a possible path to it.
            if (gameObject.tag == GlobalDefines.resourceNodeTag
             && gameObject.name.Contains(GlobalDefines.treeResourceNodeName))
            {
                float currentDistance = (job.building.transform.position - TargetObject.transform.position).sqrMagnitude;
                float newTreeDistance = (job.building.transform.position - gameObject.transform.position).sqrMagnitude;
                if (newTreeDistance < currentDistance)
                {
                    TargetObject = gameObject;

                    Stop();
                    GoToTargetObject();
                }
            }
        }

        public GameObject FindTreeNearestToBuilding()
        {
            GameObject closestTree = null;
            Vector3 buildingPos = job.building.transform.position;

            var resourceNodes = GameObject.FindGameObjectsWithTag(GlobalDefines.resourceNodeTag);
            foreach (var node in resourceNodes)
            {
                if (node.name.Contains(GlobalDefines.treeResourceNodeName))
                {
                    if (closestTree == null)
                        closestTree = node;
                    else
                    {
                        float oldDist = (buildingPos - closestTree.transform.position).sqrMagnitude;
                        float newDist = (buildingPos - node.transform.position).sqrMagnitude;
                        if (newDist < oldDist)
                        {
                            closestTree = node;
                        }
                    }
                }
            }

            return closestTree;
        }
    }
}
