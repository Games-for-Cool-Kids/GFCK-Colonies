using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jobs
{
    // TODO Extract general stuff, like going to a resource and updating if a new one is closer
    public class MoveToClosestRockTask : MoveToObjectTask
    {
        public MoveToClosestRockTask(Job job) : base(job) { }

        public override void Start()
        {
            TargetObject = FindRockNearestToBuilding();

            GameManager.GameObjectCreated += UpdateTargetRockIfCloser;

            base.Start();
        }

        public override void Finish()
        {
            GameManager.GameObjectCreated -= UpdateTargetRockIfCloser;

            base.Finish();
        }

        // TODO Needs fixing! When building a new tree, GameObjectCreated() is called for the blueprint, firing this function on a dangerously invalid tree object!
        public void UpdateTargetRockIfCloser(GameObject gameObject)
        {
            // ToDo: We should look for the closest tree that has a possible path to it.
            if (gameObject.tag == GlobalDefines.resourceNodeTag
             && gameObject.name.Contains(GlobalDefines.stoneResourceNodeName))
            {
                float currentDistance = (job.building.transform.position - TargetObject.transform.position).sqrMagnitude;
                float newRockDistance = (job.building.transform.position - gameObject.transform.position).sqrMagnitude;
                if (newRockDistance < currentDistance)
                {
                    TargetObject = gameObject;

                    Stop();
                    GoToTargetObject();
                }
            }
        }

        public GameObject FindRockNearestToBuilding()
        {
            GameObject closestRock = null;
            Vector3 buildingPos = job.building.transform.position;

            var resourceNodes = GameObject.FindGameObjectsWithTag(GlobalDefines.resourceNodeTag);
            foreach (var node in resourceNodes)
            {
                if (node.name.Contains(GlobalDefines.stoneResourceNodeName))
                {
                    if (closestRock == null)
                        closestRock = node;
                    else
                    {
                        float oldDist = (buildingPos - closestRock.transform.position).sqrMagnitude;
                        float newDist = (buildingPos - node.transform.position).sqrMagnitude;
                        if (newDist < oldDist)
                        {
                            closestRock = node;
                        }
                    }
                }
            }

            return closestRock;
        }
    }
}
