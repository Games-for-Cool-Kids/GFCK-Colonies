using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    private Vector3 moveTarget;

    // Update is called once per frame
    void Update()
    {


    }

    public void MoveToTarget (Vector3 target)
    {
        GetComponent<NavMeshAgent>().destination = target;
    }

    public void SetMoveTarget (Vector3 movetarget)
    {
        moveTarget = movetarget;
    }

    public Vector3 GetMoveTarget ()
    {
        return moveTarget;
    }
}
