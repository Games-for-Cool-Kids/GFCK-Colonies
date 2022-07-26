using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToClosestTreeTask : MoveToObjectTask
{
    public MoveToClosestTreeTask(Job job) : base(job) { }

    public override void Tick()
    {
        if(targetObject == null)
            targetObject = FindTreeNearestToBuilding();

        base.Tick();
    }

    public GameObject FindTreeNearestToBuilding()
    {
        GameObject closestTree = null;
        Vector3 buildingPos = job.building.transform.position;

        var resourceNodes = GameObject.FindGameObjectsWithTag(GlobalDefines.resourceNodeTag);
        foreach(var node in resourceNodes)
        {
            if(node.name.Contains("Tree"))
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
