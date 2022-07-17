using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    List<Block> path = null;
    int pathIndex = 0;

    public bool pingPong = false; // Unit will go back and forth along path start/end blocks.

    public float speed = 5;


    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (path == null
         || path.Count == 0)
            return;

        if (pingPong)
            PingPongPath();
        else
            FollowPath();
    }

    private void FollowPath()
    {
        if (pathIndex == path.Count) // We reached end of path
            ClearPath();

        Block targetBlock = path[pathIndex + 1];

        Vector3 characterToTarget = targetBlock.worldPosition - transform.position;
        Vector3 direction = characterToTarget.normalized;
        Vector3 move = direction * speed * Time.deltaTime;

        transform.position += move;

        float distanceToTarget = characterToTarget.magnitude;
        if (distanceToTarget < 0.1f) // If distance to center of target block is this small, we're good.
            pathIndex++;
    }

    private void PingPongPath()
    {
        int targetPathIndex = pathIndex + 1;
        if (pathIndex == path.Count) // We reached end of path
        {
            if (pingPong)
                targetPathIndex = pathIndex - 1;

        }
    }

    public void SetPath(List<Block> path)
    {
        this.path = path;
    }

    public void ClearPath()
    {
        pathIndex = 0;
        path.Clear();
    }
}
