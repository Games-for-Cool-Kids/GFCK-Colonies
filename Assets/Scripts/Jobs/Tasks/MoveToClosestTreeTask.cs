using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToClosestTreeTask : MoveToObjectTask
{
    public MoveToClosestTreeTask(Job job) : base(job) { }

    public override void Start()
    {
        targetObject = FindTreeNearestToBuilding();

        GameManager.Instance.gameObjectCreate += UpdateTargetTreeIfCloser;

        base.Start();
    }

    public void UpdateTargetTreeIfCloser(GameObject gameObject)
    {
        // ToDo: We should look for the closest tree that has a possible path to it.
        if (gameObject.tag == GlobalDefines.resourceNodeTag
         && gameObject.name.Contains(GlobalDefines.treeResourceNodeName))
        {
            float currentDistance = (job.building.transform.position - targetObject.transform.position).sqrMagnitude;
            float newTreeDistance = (job.building.transform.position - gameObject.transform.position).sqrMagnitude;
            if (newTreeDistance < currentDistance)
            {
                targetObject = gameObject;

                ClearPath();
                FindPath();
            }
        }
    }

    public GameObject FindTreeNearestToBuilding()
    {
        GameObject closestTree = null;
        Vector3 buildingPos = job.building.transform.position;

        var resourceNodes = GameObject.FindGameObjectsWithTag(GlobalDefines.resourceNodeTag);
        foreach(var node in resourceNodes)
        {
            if(node.name.Contains(GlobalDefines.treeResourceNodeName))
            {
                if (closestTree == null)
                    closestTree = node;
                else
                {
                    float oldDist = (buildingPos - closestTree.transform.position).sqrMagnitude;
                    float newDist = (buildingPos - node.transform.position).sqrMagnitude;
                    if(newDist < oldDist)
                    {
                        closestTree = node;
                    }
                }
            }
        }

        return closestTree;
    }
}
