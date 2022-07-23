using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    private List<BlockData> _path = null;
    private int _pathIndex = 0;

    public bool pingPong = false; // Unit will go back and forth along path start/end blocks.

    public float speed = 5;


    protected void Update()
    {
        FollowPath();
    }

    private void FollowPath()
    {
        if (_path == null
         || _path.Count == 0)
            return;

        BlockData targetBlock = _path[_pathIndex];

        Vector3 targetPos = targetBlock.worldPosition + GameObjectUtil.GetPivotToMeshMinOffset(gameObject) + Vector3.up / 2;
        Vector3 characterToTarget = targetPos - transform.position;
        Vector3 direction = characterToTarget.normalized;
        Vector3 move = direction * speed * Time.deltaTime;

        transform.position += move;

        float distanceToTarget = characterToTarget.magnitude;
        if (distanceToTarget < 0.1f) // If distance to center of target block is this small, we're good.
            _pathIndex++;

        if (_pathIndex >= _path.Count) // We reached end of path
        {
            _pathIndex = 0;

            if (pingPong)
                _path.Reverse();
            else
                ClearPath();
        }
    }

    public void SetPath(List<BlockData> path)
    {
        this._path = path;
    }

    public void ClearPath()
    {
        _pathIndex = 0;
        if(_path != null)
        {
            _path.Clear();
            _path = null;
        }
    }

    public BlockData GetCurrentBlock()
    {
        Vector3 posUnderBlock = transform.position - GameObjectUtil.GetPivotToMeshMinOffset(gameObject) - Vector3.up / 2; // Offset with half a block.
        return GameManager.Instance.World.GetBlockAt(posUnderBlock);
    }

    public void MoveTo(BlockData targetBlock)
    {
        ClearPath();

        Pathfinding.PathfindMaster.Instance.RequestPathfind(GetCurrentBlock(), targetBlock, SetPath);
    }
}
